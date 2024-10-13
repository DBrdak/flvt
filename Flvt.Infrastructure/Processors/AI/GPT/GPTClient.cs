using System.Runtime.CompilerServices;
using System.Text;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure.Monitoring;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Batches;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Batches.Create;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Batches.Create.Request;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Chat.Completions;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Files;
using Flvt.Infrastructure.Processors.AI.GPT.Messages;
using Flvt.Infrastructure.Processors.AI.GPT.Utils;
using Serilog;

namespace Flvt.Infrastructure.Processors.AI.GPT;

internal sealed class GPTClient
{
    private readonly HttpClient _httpClient;
    private readonly GPTMonitor _monitor;
    private readonly Dictionary<string, List<string>> _filesWithAds = [];
    private readonly Dictionary<string, List<string>> _batchesWithAds = [];

    public GPTClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _monitor = new ();
    }

    public async Task<string?> CreateCompletionAsync(IEnumerable<Message> message, GPTModel model)
    {
        var completionRequest = new CompletionCreateRequest(
            Messages: message.ToList(),
            Model: model.Value,
            ResponseFormat: GPTResponseFormats.JsonObject,
            Store: true);

        var response = await _httpClient.PostAsync(GPTPaths.CreateCompletion, CreateHttpBody(completionRequest));
        
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode) //TODO check if the error is due to rate limit
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

        _monitor.AddCompletion(completion);
        var chatResponse = completion.Choices.FirstOrDefault()?.Message.Content;

        if (chatResponse is null)
        {
            Log.Logger.Error("Failed to get chat response from GPT Completion, completion: {completion}", completion);

            return null;
        }

        return ToJson(chatResponse);
    }

    //TODO Retrieve batch > check status > return status {+ processed ads} https://platform.openai.com/docs/guides/batch/getting-started

    public async Task<Dictionary<string, List<string>>> CreateCompletionBatchesAsync(
        IEnumerable<ScrapedAdvertisement> advertisements)
    {
        var advertisementsChunks = advertisements.Chunk(GPTLimits.MaxBatchTasks / 2);

        var fileCreateTasks = advertisementsChunks.Select(CreateBatchFileAsync).ToList();

        var fileIds = await Task.WhenAll(fileCreateTasks);

        var createBatchTasks = fileIds.Select(CreateSingleBatchAsync);

        _ = await Task.WhenAll(createBatchTasks);

        return _batchesWithAds;
    }

    private async Task<Batch?> CreateSingleBatchAsync(string fileId)
    {
        var response = await _httpClient.PostAsync(
            GPTPaths.CreateBatch,
            CreateHttpBody(
                new BatchCreateRequest(
                    fileId,
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

        _batchesWithAds.Add(batch.Id, _filesWithAds[fileId]);

        return batch;
    }

    private async Task<string?> CreateBatchFileAsync(IEnumerable<ScrapedAdvertisement> advertisements)
    {
        const string purpose = "batch";

        using var form = new MultipartFormDataContent();

        var lines = advertisements.Select(ad => GPTMessageFactory.CreateBasicBatchLine(ad));

        var file = BatchFileFactory.CreateJsonlFile(lines);

        form.Add(file, "file", "@ads_batch.jsonl");
        form.Add(new StringContent(purpose), "purpose");

        var response = await _httpClient.PostAsync(GPTPaths.UploadFile, form);
        
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

        _filesWithAds.Add(createdFile.Id, advertisements.Select(ad => ad.Link).ToList());

        return createdFile.Id;
    }

    private static string ToJson(string chatResponse) =>
        chatResponse.Substring(
            chatResponse.IndexOf('{'),
            chatResponse.LastIndexOf('}') - chatResponse.IndexOf('{') + 1);

    private static StringContent CreateHttpBody<T>(T obj) => 
        new(GPTJsonConvert.Serialize(obj), Encoding.UTF8, "application/json");
}