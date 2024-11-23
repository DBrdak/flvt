using Flvt.Application.Abstractions;
using Flvt.Infrastructure.Scrapers.Domiporta;
using Flvt.Infrastructure.Scrapers.Otodom;
using Flvt.Infrastructure.Scrapers.Shared.Helpers.Constants;
using Flvt.Infrastructure.Scrapers.Shared.Helpers.Repositories;
using Serilog;

namespace Flvt.Infrastructure.Scrapers;

internal sealed class LinkScrapingOrchestrator : ILinkScrapingOrchestrator
{
    private readonly IScraperHelperRepository _scraperHelperRepository;

    public LinkScrapingOrchestrator(IScraperHelperRepository scraperHelperRepository)
    {
        _scraperHelperRepository = scraperHelperRepository;
    }

    public async Task<IEnumerable<string>> ScrapeAsync(
        string service,
        string city,
        bool onlyNew = true) =>
        service switch
        {
            Services.Domiporta => await ScrapeDomiportaAsync(city),
            Services.Otodom => await ScrapeOtodomAsync(city, onlyNew),
            _ => throw new ArgumentException($"Service: {service} does not exist")
        };

    private async Task<IEnumerable<string>> ScrapeDomiportaAsync(string city)
    {
        var latestAdvertisementHelperGetResult = await _scraperHelperRepository.GetDomiportaLatestAdvertisementHelperAsync();

        if (latestAdvertisementHelperGetResult.IsFailure)
        {
            Log.Error("Failed to retrieve {helperName}", nameof(DomiportaLatestAdvertisementHelper));
            return [];
        }

        var latestAdvertisementHelper =
            new DomiportaLatestAdvertisementHelper(latestAdvertisementHelperGetResult.Value);

        var scraper = new DomiportaLinkScraper(new(city, false), latestAdvertisementHelper);
        var result = await scraper.ScrapeAsync();

        await _scraperHelperRepository.AddRangeAsync([latestAdvertisementHelper.ToScraperHelper()]);

        return result;
    }

    private async Task<IEnumerable<string>> ScrapeOtodomAsync(string city, bool onlyNew)
    {
        var otodomScraper = new OtodomLinkScraper(new(city, onlyNew));

        var otodomTask = otodomScraper.ScrapeAsync();

        var otodomScrapeResult = otodomTask.Result;

        return [.. otodomScrapeResult];
    }
}