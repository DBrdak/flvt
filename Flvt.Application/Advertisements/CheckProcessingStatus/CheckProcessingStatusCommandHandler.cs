using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ProcessedAdvertisements;
using Flvt.Domain.ScrapedAdvertisements;

namespace Flvt.Application.Advertisements.CheckProcessingStatus;

internal sealed class CheckProcessingStatusCommandHandler : ICommandHandler<CheckProcessingResultsCommand>
{
    private readonly IProcessingOrchestrator _processingOrchestrator;
    private readonly IProcessedAdvertisementRepository _processedAdvertisementRepository;
    private readonly IScrapedAdvertisementRepository _scrapedAdvertisementRepository;

    public CheckProcessingStatusCommandHandler(
        IProcessingOrchestrator processingOrchestrator,
        IProcessedAdvertisementRepository processedAdvertisementRepository,
        IScrapedAdvertisementRepository scrapedAdvertisementRepository)
    {
        _processingOrchestrator = processingOrchestrator;
        _processedAdvertisementRepository = processedAdvertisementRepository;
        _scrapedAdvertisementRepository = scrapedAdvertisementRepository;
    }

    public async Task<Result> Handle(CheckProcessingResultsCommand request, CancellationToken cancellationToken)
    {
        var processedAdvertisements = await _processingOrchestrator.RetrieveProcessedAdvertisementsAsync();

        var links = processedAdvertisements.Select(ad => ad.Link).ToList();
        var scrapedAdvertisementsGetResult = await _scrapedAdvertisementRepository.GetManyByLinkAsync(links);

        if (scrapedAdvertisementsGetResult.IsFailure)
        {
            return scrapedAdvertisementsGetResult.Error;
        }

        scrapedAdvertisementsGetResult.Value.ToList().ForEach(a => a.Process());

        var addResult = await _processedAdvertisementRepository.AddRangeAsync(processedAdvertisements);
        var updateResult = await _scrapedAdvertisementRepository.UpdateRangeAsync(scrapedAdvertisementsGetResult.Value);

        return Result.Aggregate([addResult, updateResult]);
    }
}
