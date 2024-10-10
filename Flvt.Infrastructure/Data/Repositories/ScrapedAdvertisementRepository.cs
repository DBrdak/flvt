using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ScrapedAdvertisements;

namespace Flvt.Infrastructure.Data.Repositories;

internal sealed class ScrapedAdvertisementRepository : Repository<ScrapedAdvertisement>, IScrapedAdvertisementRepository
{
    public ScrapedAdvertisementRepository(DataContext context) : base(context)
    {
    }

    public async Task<Result<IEnumerable<ScrapedAdvertisement>>> GetManyByLinkAsync(IEnumerable<string> links) =>
        await GetManyByIdAsync(links);
}