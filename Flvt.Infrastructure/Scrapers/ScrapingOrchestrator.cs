using Flvt.Application.Abstractions;
using Flvt.Domain.Primitives.Subscribers.Filters;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure.Monitoring;
using Flvt.Infrastructure.Scrapers.Olx;
using Flvt.Infrastructure.Scrapers.Otodom;
using Serilog;

namespace Flvt.Infrastructure.Scrapers;

internal sealed class ScrapingOrchestrator : IScrapingOrchestrator
{
    private readonly ScrapingMonitor _monitor;

    public ScrapingOrchestrator(ScrapingMonitor monitor)
    {
        _monitor = monitor;
    }

    public async Task<IEnumerable<ScrapedAdvertisement>> ScrapeAsync(Filter filter)
    {
        Log.Logger.Information("Processing ads for filter: {filter}", filter);

        var otodomScraper = new OtodomScraper(filter);

        var otodomTask = otodomScraper.ScrapeAsync();

        await Task.WhenAll(otodomTask);

        var otodomAds = otodomTask.Result.ToList();

        _monitor.AddOtodom(otodomScraper);

        return [.. otodomAds];
    }
}