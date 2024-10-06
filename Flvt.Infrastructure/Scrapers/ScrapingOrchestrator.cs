using Flvt.Application.Abstractions;
using Flvt.Domain.Advertisements;
using Flvt.Domain.Subscribers;
using Flvt.Infrastructure.Scrapers.Morizon;

namespace Flvt.Infrastructure.Scrapers;

internal sealed class ScrapingOrchestrator : IScrapingOrchestrator
{

    public ScrapingOrchestrator()
    {
    }

    public async Task<IEnumerable<ScrapedAdvertisement>> ScrapeAsync(Filter filter)
    {
        var morizonScraper = new MorizonScraper(filter);
        //using var otodomScraper = new OtodomScraper(filter);
        //using var olxScraper = new OlxScraper(filter);

        var morizonTask = morizonScraper.ScrapeAsync();
        //var otodomTask = otodomScraper.ScrapeAsync();
        //var olxTask = olxScraper.ScrapeAsync();

        //await Task.WhenAll(morizonTask, otodomTask, olxTask);
        await Task.WhenAll(morizonTask);

        var morizonAdvertisements = morizonTask.Result;
        //var otodomAdvertisements = otodomTask.Result;
        //var olxAdvertisements = olxTask.Result;

        //return [..morizonAdvertisements, ..otodomAdvertisements, ..olxAdvertisements];
        return [..morizonAdvertisements];
    }
}