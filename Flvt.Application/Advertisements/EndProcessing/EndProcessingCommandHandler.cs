using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ProcessedAdvertisements;
using Flvt.Domain.ScrapedAdvertisements;

namespace Flvt.Application.Advertisements.EndProcessing;

internal sealed class EndProcessingCommandHandler : ICommandHandler<EndProcessingCommand>
{
    private readonly IProcessingOrchestrator _processingOrchestrator;
    private readonly IProcessedAdvertisementRepository _processedAdvertisementRepository;
    private readonly IScrapedAdvertisementRepository _scrapedAdvertisementRepository;

    public EndProcessingCommandHandler(
        IProcessingOrchestrator processingOrchestrator,
        IProcessedAdvertisementRepository processedAdvertisementRepository,
        IScrapedAdvertisementRepository scrapedAdvertisementRepository)
    {
        _processingOrchestrator = processingOrchestrator;
        _processedAdvertisementRepository = processedAdvertisementRepository;
        _scrapedAdvertisementRepository = scrapedAdvertisementRepository;
    }

    public async Task<Result> Handle(EndProcessingCommand request, CancellationToken cancellationToken)
    {
        var processedAdvertisements = await _processingOrchestrator.RetrieveProcessedAdvertisementsAsync();

        var links = processedAdvertisements.Select(ad => ad.Link).ToList();
        var scrapedAdvertisementsGetResult = await _scrapedAdvertisementRepository.GetManyByLinkAsync(links);

        if (scrapedAdvertisementsGetResult.IsFailure)
        {
            return scrapedAdvertisementsGetResult.Error;
        }

        //TODO Implement
        //var dedupesGetResult = await _processedAdvertisementRepository.GetAllDedupesAsync();

        //if (dedupesGetResult.IsFailure)
        //{
        //    return dedupesGetResult.Error;
        //}

        //var dedupes = dedupesGetResult.Value;

        //var adsToInsert = processedAdvertisements
        //    .ExceptBy(dedupes, ad => ad.Dedupe)
        //    .ToList();
        var adsToInsert = processedAdvertisements;

        var addResult = await _processedAdvertisementRepository.AddRangeAsync(adsToInsert);
        var updateResult = await _scrapedAdvertisementRepository.RemoveRangeAsync(scrapedAdvertisementsGetResult.Value.Select(ad => ad.Link));

        return Result.Aggregate([addResult, updateResult]);
    }
}
