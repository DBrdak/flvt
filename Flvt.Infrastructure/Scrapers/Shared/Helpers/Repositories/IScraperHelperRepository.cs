using Flvt.Domain.Primitives.Responses;

namespace Flvt.Infrastructure.Scrapers.Shared.Helpers.Repositories;

internal interface IScraperHelperRepository
{
    Task<Result<ScraperHelper>> GetDomiportaLatestAdvertisementHelperAsync();
    Task<Result> AddRangeAsync(IEnumerable<ScraperHelper> helpers);
}