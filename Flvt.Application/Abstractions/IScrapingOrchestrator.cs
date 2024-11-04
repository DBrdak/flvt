using Flvt.Domain.Filters;
using Flvt.Domain.ScrapedAdvertisements;

namespace Flvt.Application.Abstractions;

public interface IScrapingOrchestrator
{
    Task<IEnumerable<AdvertisementsScrapeResult>> ScrapeAsync(Filter filter);
}