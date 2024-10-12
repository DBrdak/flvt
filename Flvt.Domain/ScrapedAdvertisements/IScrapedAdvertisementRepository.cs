using Flvt.Domain.Primitives.Responses;

namespace Flvt.Domain.ScrapedAdvertisements;

public interface IScrapedAdvertisementRepository
{
    Task<Result<IEnumerable<ScrapedAdvertisement>>> GetAllAsync();
    Task<Result<IEnumerable<ScrapedAdvertisement>>> GetManyByLinkAsync(IEnumerable<string> links);
    Task<Result> AddRangeAsync(IEnumerable<ScrapedAdvertisement> advertisements);
}