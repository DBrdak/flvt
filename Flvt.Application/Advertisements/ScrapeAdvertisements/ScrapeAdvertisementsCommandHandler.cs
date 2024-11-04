using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Domain.Photos;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ProcessedAdvertisements;
using Flvt.Domain.ScrapedAdvertisements;

namespace Flvt.Application.Advertisements.ScrapeAdvertisements;

internal sealed class ScrapeAdvertisementsCommandHandler : ICommandHandler<ScrapeAdvertisementsCommand>
{
    private readonly IScrapingOrchestrator _scrapingOrchestrator;
    private readonly IScrapedAdvertisementRepository _scrapedAdvertisementRepository;
    private readonly IProcessedAdvertisementRepository _processedAdvertisementRepository;
    private readonly IAdvertisementPhotosRepository _advertisementPhotosRepository;

    public ScrapeAdvertisementsCommandHandler(
        IScrapingOrchestrator scrapingOrchestrator,
        IScrapedAdvertisementRepository scrapedAdvertisementRepository,
        IProcessedAdvertisementRepository processedAdvertisementRepository,
        IAdvertisementPhotosRepository advertisementPhotosRepository)
    {
        _scrapingOrchestrator = scrapingOrchestrator;
        _scrapedAdvertisementRepository = scrapedAdvertisementRepository;
        _processedAdvertisementRepository = processedAdvertisementRepository;
        _advertisementPhotosRepository = advertisementPhotosRepository;
    }

    public async Task<Result> Handle(ScrapeAdvertisementsCommand request, CancellationToken cancellationToken)
    {
        var filters = GlobalFilterFactory.CreateFiltersForAllLocations();
        var scrapeTasks = filters.Select(_scrapingOrchestrator.ScrapeAsync).ToList();
        
        var listsOfScrapedAdvertisements = await Task.WhenAll(scrapeTasks);

        var scrapedAdvertisements = listsOfScrapedAdvertisements
            .SelectMany(x => x)
            .SelectMany(x => x.ScrapedAdvertisements)
            .ToList();

        var scrapedPhotos = listsOfScrapedAdvertisements
            .SelectMany(x => x)
            .SelectMany(x => x.Photos)
            .ToList();

        var alreadyProcessedAdvertisementsGetResult = await _processedAdvertisementRepository.GetManyByLinkAsync(
                scrapedAdvertisements.Select(ad => ad.Link));

        if (alreadyProcessedAdvertisementsGetResult.IsFailure)
        {
            return await SaveScrapedAdvertisementsAsync(scrapedAdvertisements, scrapedPhotos);
        }

        var alreadyProcessedAdvertisements = alreadyProcessedAdvertisementsGetResult.Value;

        var scrapedAdvertisementsToInsert = scrapedAdvertisements
            .Where(sAd => alreadyProcessedAdvertisements.All(pAd => pAd.Link != sAd.Link))
            .ToList();

        return await SaveScrapedAdvertisementsAsync(scrapedAdvertisementsToInsert, scrapedPhotos);
    }

    private async Task<Result> SaveScrapedAdvertisementsAsync(
        List<ScrapedAdvertisement> scrapedAdvertisementsToInsert,
        List<AdvertisementPhotos> scrapedPhotos) =>
        Result.Aggregate(await Task.WhenAll(
            _scrapedAdvertisementRepository.AddRangeAsync(scrapedAdvertisementsToInsert),
            _advertisementPhotosRepository.AddRangeAsync(scrapedPhotos)));
}
