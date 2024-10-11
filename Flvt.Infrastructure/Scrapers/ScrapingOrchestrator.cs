using Flvt.Application.Abstractions;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.Primitives.Subscribers.Filters;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure.Monitoring;
using Flvt.Infrastructure.Scrapers.Morizon;
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
        var morizonScraper = new MorizonScraper(filter);
        var otodomScraper = new OtodomScraper(filter);
        var olxScraper = new OlxScraper(filter);

        var morizonTask = await morizonScraper.ScrapeAsync();
        //var otodomTask = otodomScraper.ScrapeAsync();
        //var olxTask = olxScraper.ScrapeAsync();

        //await Task.WhenAll(morizonTask, otodomTask, olxTask);

        //var morizonAds = morizonTask.Result.ToList();
        //var otodomAds = otodomTask.Result.ToList();
        //var olxAds = olxTask.Result.ToList();

        //_monitor.AddMorizon(morizonAds).AddOtodom(otodomAds).AddOlx(olxAds);

        //return [..morizonAds, .. otodomAds, .. olxAds];

        return morizonTask;
    }
}