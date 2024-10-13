using Amazon.DynamoDBv2.Model.Internal.MarshallTransformations;
using Flvt.Application.Abstractions;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ProcessedAdvertisements;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure.Data.Repositories;
using Flvt.Infrastructure.Processors.AI;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.DataModels.Batches;
using Serilog;

namespace Flvt.Infrastructure.Processors;

internal sealed class ProcessingOrchestrator : IProcessingOrchestrator
{
    private readonly IProcessedAdvertisementRepository _processedAdvertisementRepository;
    private readonly IScrapedAdvertisementRepository _scrapedAdvertisementRepository;
    private readonly BatchRepository _batchRepository;
    private readonly AIProcessor _aiProcessor;

    public ProcessingOrchestrator(
        IProcessedAdvertisementRepository processedAdvertisementRepository,
        AIProcessor aiProcessor,
        IScrapedAdvertisementRepository scrapedAdvertisementRepository,
        BatchRepository batchRepository)
    {
        _processedAdvertisementRepository = processedAdvertisementRepository;
        _aiProcessor = aiProcessor;
        _scrapedAdvertisementRepository = scrapedAdvertisementRepository;
        _batchRepository = batchRepository;
    }

    public async Task<Result<IEnumerable<ProcessedAdvertisement>>> ProcessAsync(
        IEnumerable<ScrapedAdvertisement> scrapedAdvertisements)
    {
        var processingResult = await _aiProcessor.ProcessBasicAdvertisementAsync(
            scrapedAdvertisements,
            CancellationToken.None);

        if (processingResult.IsFailure)
        {
            return processingResult.Error;
        }

        var processedAdvertisements = processingResult.Value;

        var addResult = await _processedAdvertisementRepository.AddRangeAsync(processedAdvertisements);

        if (addResult.IsFailure)
        {
            Log.Logger.Error("Failed to add processed advertisements to the database: {error}", addResult.Error);
        }

        return processedAdvertisements;
    }

    public async Task<Result> StartProcessingAsync(
        IEnumerable<ScrapedAdvertisement> scrapedAdvertisements)
    {
        var advertisementsInBatches = await _aiProcessor.StartProcessingAdvertisementsInBatchAsync(
            scrapedAdvertisements);

        var saveTasks = advertisementsInBatches.Select(pair => SaveBatchAsync(pair.Key, pair.Value));

        var results = await Task.WhenAll(saveTasks);

        return Result.Aggregate(results);
    }

    private async Task<Result> SaveBatchAsync(string batchId, List<string> advertisementsLinks)
    {
        var advertisementsGetResult =
            await _scrapedAdvertisementRepository.GetManyByLinkAsync(advertisementsLinks);

        if (advertisementsGetResult.IsFailure)
        {
            return advertisementsGetResult.Error;
        }

        var advertisements = advertisementsGetResult.Value.ToList();

        advertisements.ForEach(ad => ad.AddToBatch(batchId));

        var batchAddTask = _batchRepository.AddVoidAsync(new BatchDataModel(batchId));
        var advertisementUpdateTask = _scrapedAdvertisementRepository.UpdateRangeAsync(advertisements);

        var results = await Task.WhenAll(batchAddTask, advertisementUpdateTask);

        return Result.Aggregate(results);
    }
}