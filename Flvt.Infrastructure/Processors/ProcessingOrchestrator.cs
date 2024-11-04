using Flvt.Application.Abstractions;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ProcessedAdvertisements;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure.Data.DataModels.Batches;
using Flvt.Infrastructure.Data.Repositories;
using Flvt.Infrastructure.Processors.AI;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Batches;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Responses;
using Serilog;

namespace Flvt.Infrastructure.Processors;

internal sealed class ProcessingOrchestrator : IProcessingOrchestrator
{
    private readonly IScrapedAdvertisementRepository _scrapedAdvertisementRepository;
    private readonly BatchRepository _batchRepository;
    private readonly AIProcessor _aiProcessor;
    private const int maximumBatchSize = 500;

    public ProcessingOrchestrator(
        AIProcessor aiProcessor,
        BatchRepository batchRepository,
        IScrapedAdvertisementRepository scrapedAdvertisementRepository)
    {
        _aiProcessor = aiProcessor;
        _batchRepository = batchRepository;
        _scrapedAdvertisementRepository = scrapedAdvertisementRepository;
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

        var saveTasks = SaveBatchAsync(advertisementsInBatches);

        var saveResults = await Task.WhenAll(saveTasks);

        if (Result.Aggregate(saveResults) is var saveResult && saveResult.IsFailure)
        {
            Log.Logger.Fatal("Failed to save batch: {error}", saveResult.Error);
        }

        advertisementsInBatches
            .ForEach(
                pair => pair.AdvertisementsInBatchAsync
                    .ForEach(
                        ad => ad
                            .StartProcessing()));

        Log.Logger.Information("Started processing {count} advertisements in {batchCount} batches",
            advertisementsInBatches.SelectMany(aib => aib.AdvertisementsInBatchAsync).Count(),
            advertisementsInBatches.Select(aib => aib.BatchId).Count());

        return advertisementsInBatches.ToDictionary(
            batch => batch.BatchId,
            batch => batch.AdvertisementsInBatchAsync);
    }

    public async Task<bool> CheckIfAnyBatchFinishedAsync()
    {
        var batchesGetResult = await _batchRepository.GetUnfinishedBatchesAsync();

        if (batchesGetResult.IsFailure)
        {
            Log.Logger.Fatal("Failed to retrieve unfinished batches: {error}", batchesGetResult.Error);
            return false;
        }

        var unfinishedBatches = batchesGetResult.Value.ToList();

        var gptBatches = await _aiProcessor.RetrieveBatchesAsync(unfinishedBatches);

        var finishedBatches = gptBatches.Where(batch => batch.IsFailed || batch.IsCompleted);

        var batchesToUpdate = unfinishedBatches.Where(
            batch => finishedBatches
                .Any(finishedBatch => finishedBatch.Id == batch.Id))
            .ToList();

        Log.Logger.Information("Found {count} batches to update", batchesToUpdate.Count);

        if (!batchesToUpdate.Any())
        {
            return false;
        }

        batchesToUpdate.ForEach(batch => batch.FinishBatch());

        var updateTasks = await _batchRepository.UpdateRangeAsync(batchesToUpdate);

        if (updateTasks.IsFailure)
        {
            Log.Logger.Fatal("Failed to update finished batches: {error}", updateTasks.Error);
            return false;
        }

        return true;
    }

    public async Task<List<ProcessedAdvertisement>> RetrieveProcessedAdvertisementsAsync()
    {
        var batchesGetResult = await _batchRepository.GetFinishedBatchesAsync();

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
        var processTasks = completedBatches
            .Select(agg => _aiProcessor
                .RetrieveProcessedAdvertisements(agg.GPTBatch));

        await Task.WhenAll(deleteTasks);
        var groupedProcessedAdvertisements = await Task.WhenAll(processTasks);

        var aiProcessedAdvertisements = groupedProcessedAdvertisements
            .Where(ads => ads is not null)
            .SelectMany(ad => ad!)
            .ToList();

        await _batchRepository
            .RemoveRangeAsync(completedBatches
                .Select(batch => batch.DataBatch.Id));

        return aiProcessedAdvertisements;
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

        processingAdvertisements.ForEach(p => p.ProcessFailed());

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
                string.Concat("https://platform.openai.com/storage/files/", batch.OutputFileId, batch.ErrorFileId),
                batch.Errors);
        }

        var aggregates = from batch in failedBatches
            let batchData = batchesData.First(data => data.Id == batch.Id)
            select new BatchAggregate(
                batch,
                batchData);

        return aggregates.ToList();
    }

    private async Task<Result> SaveBatchAsync(IEnumerable<AdvertisementsBatch> batches) =>
        await _batchRepository.AddRangeAsync(
            batches.Select(
                b => new BatchDataModel(b.BatchId, b.AdvertisementsInBatchAsync.Select(ad => ad.Link))));
}