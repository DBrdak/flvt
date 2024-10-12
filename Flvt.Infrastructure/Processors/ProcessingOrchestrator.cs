using Flvt.Application.Abstractions;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ProcessedAdvertisements;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure.Processors.AI;

namespace Flvt.Infrastructure.Processors;

internal sealed class ProcessingOrchestrator : IProcessingOrchestrator
{
    private readonly IProcessedAdvertisementRepository _processedAdvertisementRepository;
    private readonly AIProcessor _aiProcessor;
    private readonly List<ProcessedAdvertisement> _processedAdvertisements = [];
    private readonly List<ScrapedAdvertisement> _advertisementsToProcess = [];

    public ProcessingOrchestrator(IProcessedAdvertisementRepository processedAdvertisementRepository, AIProcessor aiProcessor)
    {
        _processedAdvertisementRepository = processedAdvertisementRepository;
        _aiProcessor = aiProcessor;
    }

    public async Task<Result<IEnumerable<ProcessedAdvertisement>>> ProcessAsync(IEnumerable<ScrapedAdvertisement> scrapedAdvertisements)
    {
        //var existingAdvertisementsGetResult = await _processedAdvertisementRepository.GetAllAsync();
        //List<ProcessedAdvertisement> processedAdvertisements = [];

        //if (existingAdvertisementsGetResult.IsSuccess)
        //{
        //    processedAdvertisements = existingAdvertisementsGetResult.Value.ToList();
        //}

        //FindNotProcessedAdvertisements(scrapedAdvertisements, processedAdvertisements);

        //var processingResult = await _aiProcessor.ProcessBasicAdvertisementAsync(
        //    _advertisementsToProcess,
        //    CancellationToken.None);

        //if (processingResult.IsFailure)
        //{
        //    return processingResult.Error;
        //}

        //_processedAdvertisements.AddRange(processingResult.Value);

        //var addResult = await _processedAdvertisementRepository.AddRangeAsync(_processedAdvertisements);

        //if (addResult.IsFailure)
        //{
        //    Log.Logger.Error("Failed to add processed advertisements to the database: {error}", addResult.Error);
        //}

        //return _processedAdvertisements;
        return null;
    }

    //private void FindNotProcessedAdvertisements(
    //    IEnumerable<ScrapedAdvertisement> scrapedAdvertisements,
    //    List<ProcessedAdvertisement> processedAdvertisements)
    //{
    //    foreach (var scrapedAdvertisement in scrapedAdvertisements)
    //    {
    //        var processedAdvertisement = processedAdvertisements.FirstOrDefault(
    //            ad => ad.Link == scrapedAdvertisement.Link && scrapedAdvertisement.UpdatedAt == ad.UpdatedAt);

    //        switch (processedAdvertisement)
    //        {
    //            case null:
    //                _advertisementsToProcess.Add(scrapedAdvertisement);
    //                break;
    //            default:
    //                _processedAdvertisements.Add(processedAdvertisement);
    //                break;
    //        }
    //    }
    //}
}