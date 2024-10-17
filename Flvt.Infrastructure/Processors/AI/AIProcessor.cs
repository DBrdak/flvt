using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ProcessedAdvertisements;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure.Monitoring;
using Flvt.Infrastructure.Processors.AI.GPT;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Batches;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.DataModels.Batches;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Responses;
using Flvt.Infrastructure.Processors.AI.GPT.Messages;
using Flvt.Infrastructure.Processors.AI.GPT.Utils;
using Newtonsoft.Json;
using Serilog;
using Serilog.Context;

namespace Flvt.Infrastructure.Processors.AI;

internal sealed class AIProcessor
{
    private readonly GPTClient _gptClient;
    private readonly GPTMonitor _monitor;

    public AIProcessor(GPTClient gptClient, GPTMonitor monitor)
    {
        _gptClient = gptClient;
        _monitor = monitor;
    }

    public async Task<Result<List<ProcessedAdvertisement>>> ProcessBasicAdvertisementAsync(
        IEnumerable<ScrapedAdvertisement> scrapedAdvertisements)
    {
        var messages = scrapedAdvertisements.Select(GPTMessageFactory.CreateBasicMessages).ToList();
        List<Task<string?>> replyTasks = [];
        
        replyTasks.AddRange(
            messages.Select(
                message => _gptClient.CreateCompletionAsync(
                    message,
                    GPTModel.Mini4o)));

        var replyResults = await Task.WhenAll(replyTasks);

        var replies = replyResults.Where(r => r is not null);

        var processedAdvertisements = replies.Select(JsonConvert.DeserializeObject<ProcessedAdvertisement?>)
            .Where(ad => ad is not null)
            .ToList();

        return processedAdvertisements!;
    }

    public async Task<List<AdvertisementsBatch>> StartProcessingAdvertisementsInBatchAsync(
        IEnumerable<ScrapedAdvertisement> advertisements)
    {
        var advertisementsChunks = advertisements.Chunk(GPTLimits.MaxBatchTasks / 250).Take(10);//todo temp

        var fileCreateTasks = advertisementsChunks.Select(_gptClient.CreateBatchFilesAsync).ToList();

        var advertisementsFiles = await Task.WhenAll(fileCreateTasks);

        var filesForBatches = advertisementsFiles.Where(file => file is not null).Select(file => file!);

        var batches = await _gptClient.CreateBatchesFromFilesAsync(filesForBatches);

        return batches;
    }

    public async Task<List<Batch>> RetrieveBatchesAsync(IEnumerable<BatchDataModel> batches)
    {
        var batchesRetrieveTasks = batches.Select(batch => _gptClient.RetrieveBatchAsync(batch.Id));

        var batchesRetrieveResults = await Task.WhenAll(batchesRetrieveTasks);

        return batchesRetrieveResults
            .Where(batch => batch is not null)
            .Select(b => b!)
            .ToList();
    }

    public async Task<List<ProcessedAdvertisement>?> RetrieveProcessedAdvertisements(Batch batch)
    {
        using (LogContext.PushProperty("BatchId", batch.Id))
        {
            _monitor.AddBatch(batch);

            var fileContent = await _gptClient.RetrieveFileContentAsync(batch.OutputFileId);

            if (fileContent is null)
            {
                Log.Logger.Error("Failed to retrieve file content for batch");
                return null;
            }

            using var stream = new MemoryStream(fileContent);
            using var reader = new StreamReader(stream);
            List<Task<string?>> linesTasks = [];

            while (!reader.EndOfStream)
            {
                linesTasks.Add(reader.ReadLineAsync());
            }

            var lines = await Task.WhenAll(linesTasks);

            var processedAdvertisements = lines
                .Where(line => line is not null)
                .Select(l => l!)
                .Select(ReadLine);

            return processedAdvertisements
                .Where(ad => ad is not null)
                .Select(ad => ad!)
                .ToList();
        }
    }

    private ProcessedAdvertisement? ReadLine(string line)
    {
        var output = GPTJsonConvert.DeserializeObject<BatchOutput>(line);

        if (output is null)
        {
            Log.Logger.Error("Failed to deserialize GPT output line: {line}", line);
            return null;
        }

        if (!string.IsNullOrWhiteSpace(output?.Error))
        {
            Log.Logger.Error(
                "GPT failed to process completion in batch, error: {error}, output line: {line}",
                output?.Error,
                line);
        }

        var completion = ChatCompletionBatchResponse.FromBatchResponse(output?.Response);

        var response = completion?.Body.Choices.FirstOrDefault(choice => choice.Message.Role == "assistant");

        if (response is null)
        {
            Log.Logger.Error("Invalid GPT response - missing chat reply in line: {line}", line);
            return null;
        }

        _monitor.AddCompletion(completion!.Body);

        return TryDeserializeProcessedAdvertisement(response.Message.Content);
    }

    private ProcessedAdvertisement? TryDeserializeProcessedAdvertisement(string content)
    {
        try
        {
            return JsonConvert.DeserializeObject<ProcessedAdvertisement>(content) ??
                   throw new NullReferenceException(
                       "ProcessedAdvertisement deserialization returned null value");
        }
        catch (Exception e)
        {
            Log.Logger.Error(
                "Failed to deserialize ProcessedAdvertisement, content: {content}, error: {error}",
                content,
                e.Message);
            return null;
        }
    }
}