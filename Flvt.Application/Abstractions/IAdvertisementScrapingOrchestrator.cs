using Flvt.Domain.ScrapedAdvertisements;

namespace Flvt.Application.Abstractions;

public interface IAdvertisementScrapingOrchestrator
{
    Task<IEnumerable<AdvertisementsScrapeResult>> ScrapeAsync(IEnumerable<string> links);
}