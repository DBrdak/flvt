using Flvt.Application.Abstractions;
using Flvt.Domain.Advertisements;
using Flvt.Domain.Subscribers;
using Flvt.Infrastructure.Scrapers.Morizon;
using Flvt.Infrastructure.Scrapers.Otodom;

namespace Flvt.Infrastructure.Scrapers;

internal sealed class ScrapingOrchestrator : IScrapingOrchestrator
{

    public ScrapingOrchestrator()
    {
    }

    public async Task<IEnumerable<ScrapedAdvertisement>> ScrapeAsync(Filter filter)
    {
        //var morizonScraper = new MorizonScraper(filter);
        var otodomScraper = new OtodomScraper(filter);
        //var olxScraper = new OlxScraper(filter);

        //var morizonTask = morizonScraper.ScrapeAsync();
        var otodomTask = otodomScraper.ScrapeAsync();
        //var olxTask = olxScraper.ScrapeAsync();

        //await Task.WhenAll(morizonTask, otodomTask, olxTask);
        await Task.WhenAll(otodomTask);

        //var morizonAdvertisements = morizonTask.Result;
        var otodomAdvertisements = otodomTask.Result;
        //var olxAdvertisements = olxTask.Result;

        //return [..morizonAdvertisements, ..otodomAdvertisements, ..olxAdvertisements];
        return [..otodomAdvertisements];
    }
}