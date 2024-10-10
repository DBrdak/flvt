using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ProcessedAdvertisements;

namespace Flvt.Infrastructure.Data.Repositories;

internal sealed class ProcessedAdvertisementRepository : Repository<ProcessedAdvertisement>, IProcessedAdvertisementRepository
{
    public ProcessedAdvertisementRepository(DataContext context) : base(context)
    {
    }

    public async Task<Result<IEnumerable<ProcessedAdvertisement>>> GetManyByLinkAsync(
        IEnumerable<string> links) =>
        await GetManyByIdAsync(links);
}