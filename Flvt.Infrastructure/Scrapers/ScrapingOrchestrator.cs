using System.Diagnostics.CodeAnalysis;
using Flvt.Application.Abstractions;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure.Scrapers.Domiporta;
using Flvt.Infrastructure.Scrapers.Otodom;
using Flvt.Infrastructure.Scrapers.Shared.Helpers.Repositories;
using Serilog;

namespace Flvt.Infrastructure.Scrapers;

internal sealed class ScrapingOrchestrator : IScrapingOrchestrator
{
    private readonly IScraperHelperRepository _scraperHelperRepository;
    private DomiportaLatestAdvertisementHelper? _domiportaLatestAdvertisementHelper = null;

    public ScrapingOrchestrator(IScraperHelperRepository scraperHelperRepository)
    {
        _scraperHelperRepository = scraperHelperRepository;
    }

    public async Task<IEnumerable<AdvertisementsScrapeResult>> ScrapeAdvertisementsAsync(IEnumerable<string> links)
    {
        var otodomScraper = new OtodomAdvertisementScraper();
        var domiportaScraper = new DomiportaAdvertisementScraper();

        var otodomTask = otodomScraper.ScrapeAsync(links);
        var domiportaTask = domiportaScraper.ScrapeAsync(links);

        await Task.WhenAll(otodomTask, domiportaTask);

        var otodomScrapeResult = otodomTask.Result;
        var domiportaScrapeResult = domiportaTask.Result;

        return [otodomScrapeResult, domiportaScrapeResult];
    }

    public async Task<IEnumerable<string>> ScrapeLinksAsync(
        string city,
        bool onlyNew = true)
    {
        var helpersSetUp = await SetupHelpers();

        if (!helpersSetUp)
        {
            return [];
        }

        var otodomScraper = new OtodomLinkScraper(new (city, onlyNew));
        var domiportaScraper = new DomiportaLinkScraper(new(city, false), _domiportaLatestAdvertisementHelper!);

        var otodomTask = otodomScraper.ScrapeAsync();
        var domiportaTask = domiportaScraper.ScrapeAsync();

        await Task.WhenAll(otodomTask, domiportaTask);

        var otodomScrapeResult = otodomTask.Result;
        var domiportaScrapeResult = domiportaTask.Result;

        await UpdateHelpers();

        return [..otodomScrapeResult, ..domiportaScrapeResult];
    }

    [MemberNotNullWhen(
        true, 
        nameof(_domiportaLatestAdvertisementHelper)
        )]
    private async Task<bool> SetupHelpers()
    {
        var domiportaLatestAdvertisementHelperGetResult = await _scraperHelperRepository.GetDomiportaLatestAdvertisementHelperAsync();

        if (Result.Aggregate(
                [
                    domiportaLatestAdvertisementHelperGetResult
                ]
            ) is var result &&
            result.IsFailure)
        {
            Log.Error("Failed to retrieve {helperName}", nameof(DomiportaLatestAdvertisementHelper));
            return false;
        }

        var domiportaLatestAdvertisementHelper = domiportaLatestAdvertisementHelperGetResult.Value;
        _domiportaLatestAdvertisementHelper = new (domiportaLatestAdvertisementHelper);

        return true;
    }

    private async Task UpdateHelpers() =>
        await _scraperHelperRepository.AddRangeAsync([
            _domiportaLatestAdvertisementHelper!.ToScraperHelper()
        ]);

    public async Task<IEnumerable<string>> ScrapeLinks()
    {
        var helpersSetUp = await SetupHelpers();

        if (!helpersSetUp)
        {
            return [];
        }

        var domiportaScraper = new DomiportaLinkScraper(new("piaseczno", false), _domiportaLatestAdvertisementHelper!);

        var domiportaTask = domiportaScraper.ScrapeAsync();

        await Task.WhenAll(domiportaTask);

        var domiportaScrapeResult = domiportaTask.Result;

        await UpdateHelpers();

        return [.. domiportaScrapeResult];
    }

    public async Task<AdvertisementsScrapeResult> ScrapeAdvertisements(IEnumerable<string> links)
    {
        var domiportaScraper = new DomiportaAdvertisementScraper();

        var domiportaTask = domiportaScraper.ScrapeAsync(links);

        await Task.WhenAll(domiportaTask);

        var domiportaScrapeResult = domiportaTask.Result;

        return domiportaScrapeResult;
    }
}