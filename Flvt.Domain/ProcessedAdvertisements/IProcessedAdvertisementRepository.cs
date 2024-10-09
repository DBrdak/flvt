using Flvt.Domain.Primitives.Responses;

namespace Flvt.Domain.ProcessedAdvertisements;

public interface IProcessedAdvertisementRepository
{
    Task<Result<IEnumerable<ProcessedAdvertisement>>> GetAllAsync(CancellationToken cancellationToken);
    Task<Result<IEnumerable<ProcessedAdvertisement>>> GetManyByLinkAsync(IEnumerable<string> links, CancellationToken cancellationToken);
    Task<Result> AddRangeAsync(IEnumerable<ProcessedAdvertisement> advertisements, CancellationToken cancellationToken);
}