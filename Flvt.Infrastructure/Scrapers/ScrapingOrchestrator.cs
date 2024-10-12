using Flvt.Application.Abstractions;
using Flvt.Domain.Primitives.Subscribers.Filters;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure.Monitoring;
using Flvt.Infrastructure.Scrapers.Olx;
using Flvt.Infrastructure.Scrapers.Otodom;

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
        var olxScraper = new OlxScraper(filter);
        var otodomScraper = new OtodomScraper(filter);

        var olxTask = olxScraper.ScrapeAsync();
        //var otodomTask = otodomScraper.ScrapeAsync();

        //await Task.WhenAll(otodomTask, olxTask);

        //var olxAds = olxTask.Result.ToList();
        //var otodomAds = otodomTask.Result.ToList();

        //_monitor.AddOlx(morizonAds).AddOtodom(otodomAds);

        //return [..olxnAds, ..otodomAds];

        return await olxTask;
    }
}