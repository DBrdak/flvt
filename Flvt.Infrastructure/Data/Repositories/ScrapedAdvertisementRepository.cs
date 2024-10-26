using Amazon.DynamoDBv2.DocumentModel;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure.Data.DataModels;

namespace Flvt.Infrastructure.Data.Repositories;

internal sealed class ScrapedAdvertisementRepository : Repository<ScrapedAdvertisement>, IScrapedAdvertisementRepository
{
    public ScrapedAdvertisementRepository(
        DataContext context,
        DataModelService<ScrapedAdvertisement> dataModelService) : base(
        context,
        dataModelService)
    {
    }

    public async Task<Result<IEnumerable<ScrapedAdvertisement>>> GetManyByLinkAsync(IEnumerable<string> links) =>
        await GetManyByIdAsync(links);

    public async Task<Result<IEnumerable<ScrapedAdvertisement>>> GetUnprocessedAsync()
    {
        var scanFilter = new ScanFilter();
        scanFilter.AddCondition(
            nameof(ScrapedAdvertisement.IsProcessed),
            ScanOperator.Equal,
            new DynamoDBBool(false));
        scanFilter.AddCondition(
            nameof(ScrapedAdvertisement.ProcessingStartedAt),
            ScanOperator.Equal,
            new DynamoDBNull());

        return await GetWhereAsync(scanFilter);
    }

    public async Task<Result<IEnumerable<ScrapedAdvertisement>>> GetAdvertisementsInProcessAsync()
    {
        var scanFilter = new ScanFilter();
        scanFilter.AddCondition(
            nameof(ScrapedAdvertisement.IsProcessed),
            ScanOperator.Equal,
            new DynamoDBBool(false));
        scanFilter.AddCondition(
            nameof(ScrapedAdvertisement.ProcessingStartedAt),
            ScanOperator.NotEqual,
            new DynamoDBNull());

        return await GetWhereAsync(scanFilter);
    }
}