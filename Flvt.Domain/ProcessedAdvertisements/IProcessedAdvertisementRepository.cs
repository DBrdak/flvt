using Flvt.Domain.Primitives.Responses;

namespace Flvt.Domain.ProcessedAdvertisements;

public interface IProcessedAdvertisementRepository
{
    Task<Result<IEnumerable<ProcessedAdvertisement>>> GetAllAsync();
    Task<Result<IEnumerable<ProcessedAdvertisement>>> GetManyByLinkAsync(IEnumerable<string> links);
    Task<Result> AddRangeAsync(IEnumerable<ProcessedAdvertisement> advertisements);
}