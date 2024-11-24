using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Domain.Filters;
using Flvt.Domain.Primitives.Responses;

namespace Flvt.Application.Advertisements.ScrapeAdvertisementsLinks;

internal sealed class ScrapeAdvertisementsLinksCommandHandler : ICommandHandler<ScrapeAdvertisementsLinksCommand>
{
    private readonly ILinkScrapingOrchestrator _scrapingOrchestrator;
    private readonly IQueuePublisher _queuePublisher;

    public ScrapeAdvertisementsLinksCommandHandler(
        ILinkScrapingOrchestrator scrapingOrchestrator,
        IQueuePublisher queuePublisher)
    {
        _scrapingOrchestrator = scrapingOrchestrator;
        _queuePublisher = queuePublisher;
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

        return await _queuePublisher.PublishScrapedLinksAsync(links);
    }
}
