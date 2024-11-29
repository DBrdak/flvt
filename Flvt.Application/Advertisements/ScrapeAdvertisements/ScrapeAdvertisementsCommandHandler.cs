using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Domain.AdvertisementLinks;
using Flvt.Domain.Photos;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ScrapedAdvertisements;

namespace Flvt.Application.Advertisements.ScrapeAdvertisements;

internal sealed class ScrapeAdvertisementsCommandHandler : ICommandHandler<ScrapeAdvertisementsCommand>
{
    private readonly IAdvertisementScrapingOrchestrator _scrapingOrchestrator;
    private readonly IScrapedAdvertisementRepository _scrapedAdvertisementRepository;
    private readonly IAdvertisementPhotosRepository _advertisementPhotosRepository;
    private readonly IAdvertisementLinkRepository _advertisementLinkRepository;
    private const int batchSize = 512;

    public ScrapeAdvertisementsCommandHandler(
        IAdvertisementScrapingOrchestrator scrapingOrchestrator,
        IScrapedAdvertisementRepository scrapedAdvertisementRepository,
        IAdvertisementPhotosRepository advertisementPhotosRepository,
        IAdvertisementLinkRepository advertisementLinkRepository)
    {
        _scrapingOrchestrator = scrapingOrchestrator;
        _scrapedAdvertisementRepository = scrapedAdvertisementRepository;
        _advertisementPhotosRepository = advertisementPhotosRepository;
        _advertisementLinkRepository = advertisementLinkRepository;
    }

    public async Task<Result> Handle(ScrapeAdvertisementsCommand request, CancellationToken cancellationToken)
    {
        var linksGetResult = await _advertisementLinkRepository.GetAllAsync(batchSize);

        if (linksGetResult.IsFailure)
        {
            return linksGetResult.Error;
        }

        var links = linksGetResult.Value.Select(l => l.Link).ToList();

        if (!links.Any())
        {
            return Result.Success();
        }

        var alreadyScrapedLinksGetResult = await _scrapedAdvertisementRepository.GetAllLinksAsync();

        if (alreadyScrapedLinksGetResult.IsFailure)
        {
            return alreadyScrapedLinksGetResult.Error;
        }

        var alreadyScrapedLinks = alreadyScrapedLinksGetResult.Value.ToList();

        var linksToScrape = links.Except(alreadyScrapedLinks).ToList();

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
        Result.Aggregate(
            await Task.WhenAll(
                _scrapedAdvertisementRepository.AddRangeAsync(scrapedAdvertisementsToInsert),
                _advertisementPhotosRepository.AddRangeAsync(scrapedPhotos),
                _advertisementLinkRepository.RemoveRangeAsync(
                    scrapedAdvertisementsToInsert.Select(ad => ad.Link))));
}
