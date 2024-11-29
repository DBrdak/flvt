using Flvt.Domain.Primitives.Responses;

namespace Flvt.Domain.AdvertisementLinks;

public interface IAdvertisementLinkRepository
{
    Task<Result<IEnumerable<AdvertisementLink>>> GetAllAsync(int limit);

    Task<Result> AddRangeAsync(IEnumerable<AdvertisementLink> links);

    Task<Result> RemoveRangeAsync(IEnumerable<string> links);
}