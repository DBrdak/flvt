using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Domain.Photos;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ScrapedAdvertisements;

namespace Flvt.Application.Advertisements.ScrapeAdvertisements;

internal sealed class ScrapeAdvertisementsCommandHandler : ICommandHandler<ScrapeAdvertisementsCommand>
{
    private readonly IAdvertisementScrapingOrchestrator _scrapingOrchestrator;
    private readonly IScrapedAdvertisementRepository _scrapedAdvertisementRepository;
    private readonly IAdvertisementPhotosRepository _advertisementPhotosRepository;

    public ScrapeAdvertisementsCommandHandler(
        IAdvertisementScrapingOrchestrator scrapingOrchestrator,
        IScrapedAdvertisementRepository scrapedAdvertisementRepository,
        IAdvertisementPhotosRepository advertisementPhotosRepository)
    {
        _scrapingOrchestrator = scrapingOrchestrator;
        _scrapedAdvertisementRepository = scrapedAdvertisementRepository;
        _advertisementPhotosRepository = advertisementPhotosRepository;
    }

    public async Task<Result> Handle(ScrapeAdvertisementsCommand request, CancellationToken cancellationToken)
    {
        var alreadyScrapedAdvertisementsLinksGetResult = await _scrapedAdvertisementRepository.GetManyByLinkAsync(request.Links);

        if (alreadyScrapedAdvertisementsLinksGetResult.IsFailure)
        {
            return alreadyScrapedAdvertisementsLinksGetResult.Error;
        }

        var alreadyScrapedAdvertisementsLinks = alreadyScrapedAdvertisementsLinksGetResult.Value;

        var alreadyScrapedLinks = alreadyScrapedAdvertisementsLinks.Select(ad => ad.Link).ToList();

        var linksToScrape = request.Links.Except(alreadyScrapedLinks).ToList();

        var scrapeResult = (await _scrapingOrchestrator.ScrapeAsync(linksToScrape)).ToList();

        var scrapedAdvertisements = scrapeResult
            .Select(x => x)
            .SelectMany(x => x.ScrapedAdvertisements)
            .ToList();

        var scrapedPhotos = scrapeResult
            .Select(x => x)
            .SelectMany(x => x.Photos)
            .ToList();

        return await SaveScrapedAdvertisementsAsync(scrapedAdvertisements, scrapedPhotos);
    }

    private async Task<Result> SaveScrapedAdvertisementsAsync(
        List<ScrapedAdvertisement> scrapedAdvertisementsToInsert,
        List<AdvertisementPhotos> scrapedPhotos) =>
        Result.Aggregate(await Task.WhenAll(
            _scrapedAdvertisementRepository.AddRangeAsync(scrapedAdvertisementsToInsert),
            _advertisementPhotosRepository.AddRangeAsync(scrapedPhotos)));
}
