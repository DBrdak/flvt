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
        Func<Task<DomiportaLatestAdvertisementHelper?>> getAdvertisementHelper = async () =>
        {
            var latestAdvertisementHelperGetResult =
                await _scraperHelperRepository.GetDomiportaLatestAdvertisementHelperAsync();

            if (latestAdvertisementHelperGetResult.IsSuccess)
            {
                return new(latestAdvertisementHelperGetResult.Value, city);
            }

            Log.Error("Failed to retrieve {helperName}", nameof(DomiportaLatestAdvertisementHelper));
            return null;
        };

        var latestAdvertisementHelper = await getAdvertisementHelper();

        if (latestAdvertisementHelper is null)
        {
            return [];
        }

        var scraper = new DomiportaLinkScraper(new(city, false), latestAdvertisementHelper);
        var result = await scraper.ScrapeAsync();

        latestAdvertisementHelper = await getAdvertisementHelper();

        if (latestAdvertisementHelper is null)
        {
            return [];
        }

        await _scraperHelperRepository.AddAsync(latestAdvertisementHelper.ToScraperHelper());

        return result;
    }

    private async Task<IEnumerable<string>> ScrapeOtodomAsync(string city, bool onlyNew)
    {
        var scraper = new OtodomLinkScraper(new(city, onlyNew));

        var result = await scraper.ScrapeAsync();

        return result;
    }
}