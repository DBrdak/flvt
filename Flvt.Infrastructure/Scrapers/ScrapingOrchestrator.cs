using Flvt.Application.Abstractions;
using Flvt.Domain.Filters;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure.Scrapers.Otodom;

namespace Flvt.Infrastructure.Scrapers;

internal sealed class ScrapingOrchestrator : IScrapingOrchestrator
{
    public async Task<IEnumerable<AdvertisementsScrapeResult>> ScrapeAdvertisementsAsync(IEnumerable<string> links)
    {
        var otodomScraper = new OtodomAdvertisementScraper();

        var otodomTask = otodomScraper.ScrapeAsync(links);

        await Task.WhenAll(otodomTask);

        var otodomScrapeResult = otodomTask.Result;

        return [otodomScrapeResult];
    }

    public async Task<IEnumerable<string>> ScrapeLinksAsync(
        string city,
        bool onlyNew = true)
    {
        var otodomScraper = new OtodomLinkScraper(new (city, onlyNew));

        var otodomTask = otodomScraper.ScrapeAsync();

        await Task.WhenAll(otodomTask);

        var otodomScrapeResult = otodomTask.Result;

        return [..otodomScrapeResult];
    }
}