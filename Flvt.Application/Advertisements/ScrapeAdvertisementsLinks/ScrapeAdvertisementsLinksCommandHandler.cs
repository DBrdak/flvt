using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Domain.AdvertisementLinks;
using Flvt.Domain.Filters;
using Flvt.Domain.Primitives.Responses;

namespace Flvt.Application.Advertisements.ScrapeAdvertisementsLinks;

internal sealed class ScrapeAdvertisementsLinksCommandHandler : ICommandHandler<ScrapeAdvertisementsLinksCommand>
{
    private readonly ILinkScrapingOrchestrator _scrapingOrchestrator;
    private readonly IAdvertisementLinkRepository _advertisementLinkRepository;

    public ScrapeAdvertisementsLinksCommandHandler(
        ILinkScrapingOrchestrator scrapingOrchestrator,
        IAdvertisementLinkRepository advertisementLinkRepository)
    {
        _scrapingOrchestrator = scrapingOrchestrator;
        _advertisementLinkRepository = advertisementLinkRepository;
    }

    public async Task<Result> Handle(ScrapeAdvertisementsLinksCommand request, CancellationToken cancellationToken)
    {
        var cities = FilterLocation.SupportedCities;

        var linksScrapeTasks = cities
            .Select(city => _scrapingOrchestrator.ScrapeAsync(request.Service, city.City))
            .ToList();

        await Task.WhenAll(linksScrapeTasks);

        var linksScrapeResults = linksScrapeTasks.Select(task => task.Result).ToList();

        var links = linksScrapeResults.SelectMany(x => x).ToList();

        return await _advertisementLinkRepository.AddRangeAsync(links.Select(l => new AdvertisementLink(l)));
    }
}
