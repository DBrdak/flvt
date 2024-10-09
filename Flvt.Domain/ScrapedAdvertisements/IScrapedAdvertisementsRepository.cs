using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ScrapedAdvertisements;

namespace Flvt.Domain.ScrapedAdvertisements;

public interface IScrapedAdvertisementsRepository
{
    Task<Result<IEnumerable<ScrapedAdvertisement>>> GetAllAsync(CancellationToken cancellationToken);
    Task<Result<IEnumerable<ScrapedAdvertisement>>> GetManyByLinkAsync(IEnumerable<string> links, CancellationToken cancellationToken);
    Task<Result> AddRangeAsync(IEnumerable<ScrapedAdvertisement> advertisements);
}