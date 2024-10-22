using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ProcessedAdvertisements;
using Flvt.Domain.ScrapedAdvertisements;

namespace Flvt.Application.Advertisements.ScrapeAdvertisements;

internal sealed class ScrapeAdvertisementsCommandHandler : ICommandHandler<ScrapeAdvertisementsCommand>
{
    private readonly IScrapingOrchestrator _scrapingOrchestrator;
    private readonly IScrapedAdvertisementRepository _scrapedAdvertisementRepository;
    private readonly IProcessedAdvertisementRepository _processedAdvertisementRepository;

    public ScrapeAdvertisementsCommandHandler(
        IScrapingOrchestrator scrapingOrchestrator,
        IScrapedAdvertisementRepository scrapedAdvertisementRepository,
        IProcessedAdvertisementRepository processedAdvertisementRepository)
    {
        _scrapingOrchestrator = scrapingOrchestrator;
        _scrapedAdvertisementRepository = scrapedAdvertisementRepository;
        _processedAdvertisementRepository = processedAdvertisementRepository;
    }

    public async Task<Result> Handle(ScrapeAdvertisementsCommand request, CancellationToken cancellationToken)
    {
        var filters = GlobalFilterFactory.CreateFiltersForAllLocations();
        var scrapeTasks = filters.Select(_scrapingOrchestrator.ScrapeAsync).ToList();
        
        var listsOfScrapedAdvertisements = await Task.WhenAll(scrapeTasks);

        var scrapedAdvertisements = listsOfScrapedAdvertisements
            .SelectMany(x => x)
            .ToList();

        _processedAdvertisementRepository.StartBatchGet();
        
        scrapedAdvertisements
            .Select(x => x.Link)
            .ToList()
            .ForEach(_processedAdvertisementRepository.AddItemToBatchGet);

        var alreadyProcessedAdvertisementsGetResult = await _processedAdvertisementRepository.ExecuteBatchGetAsync();

        if (alreadyProcessedAdvertisementsGetResult.IsFailure)
        {
            return await SaveScrapedAdvertisementsAsync(scrapedAdvertisements);
        }

        var alreadyProcessedAdvertisements = alreadyProcessedAdvertisementsGetResult.Value;

        var scrapedAdvertisementsToInsert = scrapedAdvertisements
            .Where(sAd => alreadyProcessedAdvertisements.All(pAd => pAd.Link != sAd.Link))
            .ToList();

        return await SaveScrapedAdvertisementsAsync(scrapedAdvertisementsToInsert);
    }

    private async Task<Result> SaveScrapedAdvertisementsAsync(
        List<ScrapedAdvertisement> scrapedAdvertisementsToInsert)
    {
        _scrapedAdvertisementRepository.StartBatchWrite();

        scrapedAdvertisementsToInsert
            .ForEach(_scrapedAdvertisementRepository.AddItemToBatchWrite);

        return await _scrapedAdvertisementRepository.ExecuteBatchWriteAsync();
    }
}
