using Flvt.Domain.Advertisements;
using Flvt.Domain.Subscribers;

namespace Flvt.Application.Abstractions;

public interface IScrapingOrchestrator
{
    Task<IEnumerable<ScrapedAdvertisement>> ScrapeAsync(Filter filter);
}