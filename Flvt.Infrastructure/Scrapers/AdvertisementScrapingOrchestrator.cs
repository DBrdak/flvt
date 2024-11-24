using Flvt.Application.Abstractions;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure.Scrapers.Domiporta;
using Flvt.Infrastructure.Scrapers.Otodom;
using Flvt.Infrastructure.Scrapers.Shared.Helpers.Constants;
using Serilog;

namespace Flvt.Infrastructure.Scrapers;

internal class AdvertisementScrapingOrchestrator : IAdvertisementScrapingOrchestrator
{
    public async Task<IEnumerable<AdvertisementsScrapeResult>> ScrapeAsync(IEnumerable<string> links)
    {
        var linksDictionary = links.GroupBy(
            link => link switch
            {
                _ when link.Contains(Services.Otodom.ToLower()) => Services.Otodom,
                _ when link.Contains(Services.Domiporta.ToLower()) => Services.Domiporta,
                _ => "Unknown"
            })
            .ToDictionary(group => group.Key, group => group.ToList());

        if (linksDictionary.TryGetValue("Unknown", out var unknownAds))
        {
            Log.Logger.Error(
                "Identified {unknownAdsCount} unkown ads when trying to scrap advertisements",
                unknownAds.Count);
        }

        return await Task.WhenAll(
            ScrapeDomiportaAsync(linksDictionary.GetValueOrDefault(Services.Domiporta)?.ToList() ?? []),
            ScrapeOtodomAsync(linksDictionary.GetValueOrDefault(Services.Otodom)?.ToList() ?? [])
        );
    }

    private async Task<AdvertisementsScrapeResult> ScrapeOtodomAsync(List<string> links) => 
        await new OtodomAdvertisementScraper().ScrapeAsync(links);
    private async Task<AdvertisementsScrapeResult> ScrapeDomiportaAsync(List<string> links) => 
        await new DomiportaAdvertisementScraper().ScrapeAsync(links);
}