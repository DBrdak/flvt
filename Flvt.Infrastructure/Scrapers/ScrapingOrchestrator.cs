using Flvt.Application.Abstractions;
using Flvt.Domain.Filters;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure.Scrapers.Otodom;

namespace Flvt.Infrastructure.Scrapers;

internal sealed class ScrapingOrchestrator : IScrapingOrchestrator
{
    public async Task<IEnumerable<AdvertisementsScrapeResult>> ScrapeAsync(Filter filter)
    {
        var otodomScraper = new OtodomScraper(filter);

        var otodomTask = otodomScraper.ScrapeAsync();

        await Task.WhenAll(otodomTask);

        var otodomScrapeResult = otodomTask.Result;

        return [otodomScrapeResult];
    }
}