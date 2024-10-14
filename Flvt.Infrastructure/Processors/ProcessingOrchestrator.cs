using Amazon.DynamoDBv2.Model.Internal.MarshallTransformations;
using Flvt.Application.Abstractions;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ProcessedAdvertisements;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure.Data.Repositories;
using Flvt.Infrastructure.Processors.AI;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Batches;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.DataModels.Batches;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Responses;
using Serilog;

namespace Flvt.Infrastructure.Processors;

internal sealed class ProcessingOrchestrator : IProcessingOrchestrator
{
    private readonly IScrapedAdvertisementRepository _scrapedAdvertisementRepository;
    private readonly IProcessedAdvertisementRepository _processedAdvertisementRepository;
    private readonly BatchRepository _batchRepository;
    private readonly AIProcessor _aiProcessor;
    private const int maximumBatchSize = 500;

    public ProcessingOrchestrator(
        AIProcessor aiProcessor,
        BatchRepository batchRepository,
        IScrapedAdvertisementRepository scrapedAdvertisementRepository,
        IProcessedAdvertisementRepository processedAdvertisementRepository)
    {
        _aiProcessor = aiProcessor;
        _batchRepository = batchRepository;
        _scrapedAdvertisementRepository = scrapedAdvertisementRepository;
        _processedAdvertisementRepository = processedAdvertisementRepository;
    }

    public async Task<Result<List<ProcessedAdvertisement>>> ProcessAsync(
        IEnumerable<ScrapedAdvertisement> scrapedAdvertisements)
    {
        var advertisementsChunks = scrapedAdvertisements.Chunk(maximumBatchSize);

        var processingTasks = advertisementsChunks.Select(_aiProcessor.ProcessBasicAdvertisementAsync);
        var processingResults = await Task.WhenAll(processingTasks);

        if (Result.Aggregate(processingResults) is var processingResult && processingResult.IsFailure)
        {
            return processingResult.Error;
        }

        var processedAdvertisements = processingResults.SelectMany(result => result.Value).ToList();

        return Result.Create(processedAdvertisements);
    }

    public async Task<Dictionary<string, List<ScrapedAdvertisement>>> StartProcessingAsync(
        IEnumerable<ScrapedAdvertisement> scrapedAdvertisements)
    {
        var advertisementsInBatches = await _aiProcessor.StartProcessingAdvertisementsInBatchAsync(scrapedAdvertisements);

        var saveTasks = advertisementsInBatches.Select(SaveBatchAsync);

        var saveResults = await Task.WhenAll(saveTasks);

        if (Result.Aggregate(saveResults) is var saveResult && saveResult.IsFailure)
        {
            Log.Logger.Fatal("Failed to save batch: {error}", saveResult.Error);
        }

        return advertisementsInBatches.ToDictionary(
            batch => batch.BatchId,
            batch => batch.AdvertisementsInBatchAsync);
    }

    public async Task<List<ProcessedAdvertisement>> RetrieveProcessedAdvertisementsAsync()
    {
        var batchesGetResult = await _batchRepository.GetAllAsync();

        if (batchesGetResult.IsFailure)
        {
            Log.Logger.Fatal("Failed to retrieve batches: {error}", batchesGetResult.Error);
            return [];
        }

        var dataBatches = batchesGetResult.Value.ToList();

        var gptBatches = await _aiProcessor.RetrieveBatchesAsync(dataBatches);

        var completedBatches = GetCompletedBatches(gptBatches, dataBatches);
        var failedBatchesIds = GetAndReportFailedBatches(gptBatches, dataBatches);

        var deleteTasks = failedBatchesIds.Select(RemoveFailedBatchAsync);
        var processTasks =
            completedBatches.Select(agg => _aiProcessor.RetrieveProcessedAdvertisements(agg.GPTBatch));

        await Task.WhenAll(deleteTasks);
        var groupedProcessedAdvertisements = await Task.WhenAll(processTasks);
        var processedAdvertisements = groupedProcessedAdvertisements
            .Where(ads => ads is not null)
            .SelectMany(ad => ad!)
            .ToList();

        await _batchRepository.RemoveRangeAsync(completedBatches.Select(batch => batch.DataBatch.Id));

        return processedAdvertisements;
    }

    private async Task RemoveFailedBatchAsync(BatchAggregate batchAggregate)
    {
        var processingAdvertisementsGetResult =
            await _scrapedAdvertisementRepository.GetManyByLinkAsync(
                batchAggregate.DataBatch.ProcessingAdvertisementsLinks);

        if (processingAdvertisementsGetResult.IsFailure)
        {
            Log.Logger.Error(
                "Failed to retrieve advertisements processed by failed batch {batchId}, error: {error}",
                batchAggregate.DataBatch.Id,
                processingAdvertisementsGetResult.Error);
            return;
        }

        var processingAdvertisements = processingAdvertisementsGetResult.Value.ToList();

        processingAdvertisements.ForEach(p => p.Process());

        var advertisementsUpdateResult = await _scrapedAdvertisementRepository.UpdateRangeAsync(processingAdvertisements);

        if (advertisementsUpdateResult.IsFailure)
        {
            Log.Logger.Fatal(
                "Failed to update advertisements processed by failed batch {batchId}, error: {error}",
                batchAggregate.DataBatch.Id,
                advertisementsUpdateResult.Error);
            return;
        }

        var batchRemoveResult = await _batchRepository.RemoveAsync(batchAggregate.DataBatch.Id);

        if (batchRemoveResult.IsFailure)
        {
            Log.Logger.Error(
                "Failed to remove failed batch {batchId}, error: {error}",
                batchAggregate.DataBatch.Id,
                batchRemoveResult.Error);
        }
    }

    private List<BatchAggregate> GetCompletedBatches(List<Batch> batches, List<BatchDataModel> batchesData)
    {
        var completedBatches = batches.Where(batch => batch.IsCompleted).ToList();

        var aggregates =  from batch in completedBatches
            let batchData = batchesData.First(data => data.Id == batch.Id)
            select new BatchAggregate(
                batch,
                batchData);

        return aggregates.ToList();
    }

    private List<BatchAggregate> GetAndReportFailedBatches(List<Batch> batches, List<BatchDataModel> batchesData)
    {
        var failedBatches = batches.Where(batch => batch.IsFailed).ToList();

        foreach (var batch in failedBatches)
        {
            Log.Logger.Error(
                "Batch {batchId} failed to file {fileId} with error: {error}",
                batch.Id,
                string.Concat("https://platform.openai.com/storage/files/", batch.ErrorFileId),
                batch.Errors);
        }

        var aggregates = from batch in failedBatches
            let batchData = batchesData.First(data => data.Id == batch.Id)
            select new BatchAggregate(
                batch,
                batchData);

        return aggregates.ToList();
    }

    private async Task<Result> SaveBatchAsync(AdvertisementsBatch batch) =>
        await _batchRepository.AddVoidAsync(
            new(
                batch.BatchId,
                batch.AdvertisementsInBatchAsync.Select(ad => ad.Link)));
}