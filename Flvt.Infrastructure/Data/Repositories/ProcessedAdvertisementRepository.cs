using Flvt.Domain.Advertisements;
using Flvt.Domain.Advertisements.Errors;
using Flvt.Domain.Primitives;

namespace Flvt.Infrastructure.Data.Repositories;

internal sealed class ProcessedAdvertisementRepository : Repository<ProcessedAdvertisement>, IProcessedAdvertisementRepository
{
    public ProcessedAdvertisementRepository(DataContext context) : base(context)
    {
    }

    public async Task<Result<IEnumerable<ProcessedAdvertisement>>> GetManyByLinkAsync(
        IEnumerable<string> links,
        CancellationToken cancellationToken) =>
        await GetManyByIdAsync(links, cancellationToken);
}