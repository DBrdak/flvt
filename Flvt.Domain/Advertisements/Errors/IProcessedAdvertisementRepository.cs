using Flvt.Domain.Primitives;

namespace Flvt.Domain.Advertisements.Errors;

public interface IProcessedAdvertisementRepository
{
    Task<Result<IEnumerable<ProcessedAdvertisement>>> GetAllAsync(CancellationToken cancellationToken);
    Task<Result<IEnumerable<ProcessedAdvertisement>>> GetManyByLinkAsync(IEnumerable<string> links, CancellationToken cancellationToken);
    Task<Result> AddRangeAsync(IEnumerable<ProcessedAdvertisement> advertisements, CancellationToken cancellationToken);
}