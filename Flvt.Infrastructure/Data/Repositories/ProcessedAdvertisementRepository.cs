using Amazon.DynamoDBv2.DocumentModel;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ProcessedAdvertisements;
using Filter = Flvt.Domain.Filters.Filter;

namespace Flvt.Infrastructure.Data.Repositories;

internal sealed class ProcessedAdvertisementRepository : Repository<ProcessedAdvertisement>, IProcessedAdvertisementRepository
{
    public ProcessedAdvertisementRepository(DataContext context) : base(context)
    {
    }

    public async Task<Result<IEnumerable<ProcessedAdvertisement>>> GetManyByLinkAsync(
        IEnumerable<string> links) =>
        await GetManyByIdAsync(links);

    public async Task<Result<ProcessedAdvertisement>> GetByLinkAsync(string link) =>
        await GetByIdAsync(link);

    //TODO Implement
    public async Task<Result<IEnumerable<ProcessedAdvertisement>>> GetByFilterAsync(Filter filter)
    {
        throw new NotImplementedException();
        //var scanFilter = new ScanFilter();
        //scanFilter.AddCondition(
        //    "Address.City",
        //    ScanOperator.Equal,
        //    filter.Location.City);
        //scanFilter.AddCondition(
        //    "Price.Amount",
        //    ScanOperator.GreaterThanOrEqual,
        //    filter.MinPrice?.Value);
        //scanFilter.AddCondition(
        //    "Price.Amount",
        //    ScanOperator.LessThanOrEqual,
        //    filter.MaxPrice?.Value);

        //return await GetWhereAsync(scanFilter);
    }
}