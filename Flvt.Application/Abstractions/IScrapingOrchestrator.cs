using Flvt.Domain.Advertisements;

namespace Flvt.Application.Abstractions;

public interface IScrapingOrchestrator
{
    Task<IEnumerable<ScrapedAdvertisement>> ScrapeAsync(Filter filter);
}