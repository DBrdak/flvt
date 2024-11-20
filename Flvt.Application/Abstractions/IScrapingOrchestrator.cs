using Flvt.Domain.ScrapedAdvertisements;

namespace Flvt.Application.Abstractions;

public interface IScrapingOrchestrator
{
    Task<IEnumerable<AdvertisementsScrapeResult>> ScrapeAdvertisementsAsync(IEnumerable<string> links);

    Task<IEnumerable<string>> ScrapeLinksAsync(string city, bool onlyNew = true);

    Task<IEnumerable<string>> ScrapeLinks();

    Task<AdvertisementsScrapeResult> ScrapeAdvertisements(IEnumerable<string> links);
}