using Amazon.DynamoDBv2.DocumentModel;
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

    public async Task<Result<IEnumerable<ScrapedAdvertisement>>> GetUnprocessedAsync()
    {
        var scanFilter = new ScanFilter();
        scanFilter.AddCondition(nameof(ScrapedAdvertisement.IsProcessed), ScanOperator.Equal, false);
        scanFilter.AddCondition(nameof(ScrapedAdvertisement.ProcessingStartedAt), ScanOperator.IsNull);
        scanFilter.AddCondition(nameof(ScrapedAdvertisement.BatchId), ScanOperator.IsNull);

        return await GetWhereAsync(scanFilter);
    }
}