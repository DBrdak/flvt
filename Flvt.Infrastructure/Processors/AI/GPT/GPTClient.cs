using System.Text;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Batches;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Batches.Create;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Batches.Create.Request;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Chat.Completions;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Files;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Responses;
using Flvt.Infrastructure.Processors.AI.GPT.Messages;
using Flvt.Infrastructure.Processors.AI.GPT.Utils;
using Flvt.Infrastructure.Utlis.Extensions;
using Serilog;

namespace Flvt.Infrastructure.Processors.AI.GPT;

internal sealed class GPTClient
{
    private readonly HttpClient _httpClient;

    public GPTClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> CreateCompletionAsync(IEnumerable<Message> message, GPTModel model)
    {
        var completionRequest = new CompletionCreateRequest(
            Messages: message.ToList(),
            Model: model.Value,
            ResponseFormat: GPTResponseFormats.JsonObject,
            TopP: GPTFineTuneDefaults.HighTopP,
            Temperature: GPTFineTuneDefaults.LowTemperature,
            Store: true);

        var response = await _httpClient.TryPostAsync(GPTPaths.CreateCompletion, CreateHttpBody(completionRequest));
        
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Log.Logger.Error("Failed to create GPT Completion, response: {response}", responseContent);

            return null;
        }

        var completion = GPTJsonConvert.DeserializeObject<ChatCompletion>(responseContent);

        if (completion is null)
        {
            Log.Logger.Error("Failed to deserialize GPT Completion on response: {response}", responseContent);

            return null;
        }

        var chatResponse = completion.Choices.FirstOrDefault()?.Message.Content;

        if (chatResponse is null)
        {
            Log.Logger.Error("Failed to get chat response from GPT Completion, completion: {completion}", completion);

            return null;
        }

        return ToJson(chatResponse);
    }

    public async Task<Batch?> RetrieveBatchAsync(string batchId)
    {
        var response = await _httpClient.TryGetAsync(GPTPaths.RetrieveBatch(batchId));

        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Log.Logger.Error("Failed to retrieve GPT Batch, response: {response}", responseContent);

            return null;
        }

        var batch = GPTJsonConvert.DeserializeObject<Batch>(responseContent);

        if (batch is null)
        {
            Log.Logger.Error("Failed to deserialize GPT Batch on response: {response}", responseContent);

            return null;
        }

        return batch;
    }

    public async Task<List<AdvertisementsBatch>> CreateBatchesFromFilesAsync(
        IEnumerable<AdvertisementsFile> files)
    {
        var createBatchTasks = files.Select(CreateSingleBatchAsync);

        var advertisementsBatches = await Task.WhenAll(createBatchTasks);

        return advertisementsBatches.Where(b => b is not null).Select(b => b!).ToList();
    }

    private async Task<AdvertisementsBatch?> CreateSingleBatchAsync(AdvertisementsFile file)
    {
        var response = await _httpClient.TryPostAsync(
            GPTPaths.CreateBatch,
            CreateHttpBody(
                new BatchCreateRequest(
                    file.FileId,
                    new Dictionary<string, string>
                        { { "Type", "Basic" } })));

        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Log.Logger.Error("Failed to create GPT Batch, response: {response}", responseContent);

            return null;
        }

        var batch = GPTJsonConvert.DeserializeObject<Batch>(responseContent);

        if (batch is null)
        {
            Log.Logger.Error("Failed to deserialize GPT Batch on response: {response}", responseContent);

            return null;
        }

        return new(batch.Id, file.AdvertisementsInFileAsync);
    }

    public async Task<AdvertisementsFile?> CreateBatchFilesAsync(
        IEnumerable<ScrapedAdvertisement> advertisements)
    {
        var ads = advertisements.ToList();

        if (ads.Count > GPTLimits.MaxBatchTasks)
        {
            throw new ArgumentOutOfRangeException(
                nameof(advertisements),
                $"The number of advertisements is greater than the maximum allowed: {GPTLimits.MaxBatchTasks}");
        }

        const string purpose = "batch";

        using var form = new MultipartFormDataContent();

        var lines = ads.Select(ad => GPTMessageFactory.CreateBasicBatchLine(ad));

        var file = BatchFileFactory.CreateJsonlFile(lines);

        form.Add(file, "file", $"{DateTime.UtcNow:dd/MM/yyyy HH:mm:ss.fff}.jsonl");
        form.Add(new StringContent(purpose), "purpose");

        var response = await _httpClient.TryPostAsync(GPTPaths.UploadFile, form);
        
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Log.Logger.Error("Failed to create GPT File, response: {response}", responseContent);

            return null;
        }

        var createdFile = GPTJsonConvert.DeserializeObject<FileModel>(responseContent);

        if (createdFile is null)
        {
            Log.Logger.Error("Failed to deserialize GPT File on response: {response}", responseContent);

            return null;
        }

        return new(createdFile.Id, ads);
    }

    public async Task<byte[]?> RetrieveFileContentAsync(string fileId)
    {
        var response = await _httpClient.TryGetAsync(GPTPaths.RetrieveFileContent(fileId));

        var responseContent = await response.Content.ReadAsByteArrayAsync();

        if (!response.IsSuccessStatusCode)
        {
            Log.Logger.Error("Failed to retrieve GPT File content, response: {response}", responseContent);

            return null;
        }

        return responseContent;
    }

    private static string ToJson(string chatResponse) =>
        chatResponse.Substring(
            chatResponse.IndexOf('{'),
            chatResponse.LastIndexOf('}') - chatResponse.IndexOf('{') + 1);

    private static StringContent CreateHttpBody<T>(T obj) => 
        new(GPTJsonConvert.Serialize(obj), Encoding.UTF8, "application/json");
}