using Amazon.DynamoDBv2.DocumentModel;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ProcessedAdvertisements;
using Flvt.Infrastructure.Data.DataModels;
using Flvt.Infrastructure.Data.DataModels.ProcessedAdvertisements;
using Filter = Flvt.Domain.Filters.Filter;

namespace Flvt.Infrastructure.Data.Repositories;

internal sealed class ProcessedAdvertisementRepository : Repository<ProcessedAdvertisement>, IProcessedAdvertisementRepository
{
    public ProcessedAdvertisementRepository(
        DataContext context,
        DataModelService<ProcessedAdvertisement> dataModelService) : base(
        context,
        dataModelService)
    {
    }

    public async Task<Result<IEnumerable<ProcessedAdvertisement>>> GetManyByLinkAsync(
        IEnumerable<string> links) =>
        await GetManyByIdAsync(links);

    public async Task<Result<ProcessedAdvertisement>> GetByLinkAsync(string link) =>
        await GetByIdAsync(link);

    public async Task<Result<IEnumerable<ProcessedAdvertisement>>> GetByFilterAsync(Filter filter)
    {
        var scanFilter = new ScanFilter();
        scanFilter.AddCondition(
            nameof(ProcessedAdvertisementDataModel.AddressCity),
            ScanOperator.Equal,
            filter.Location.City);

        if (filter.MinArea?.Value > 0)
        {
            scanFilter.AddCondition(
                nameof(ProcessedAdvertisementDataModel.AreaAmount),
                ScanOperator.GreaterThanOrEqual,
                filter.MinArea.Value);
        }
        if (filter.MaxArea?.Value > 0)
        {
            scanFilter.AddCondition(
                nameof(ProcessedAdvertisementDataModel.AreaAmount),
                ScanOperator.LessThanOrEqual,
                filter.MaxArea.Value);
        }
        if (filter.MinPrice?.Value > 0)
        {
            scanFilter.AddCondition(
                nameof(ProcessedAdvertisementDataModel.PriceAmount),
                ScanOperator.GreaterThanOrEqual,
                filter.MinPrice.Value);
        }
        if (filter.MaxPrice?.Value > 0)
        {
            scanFilter.AddCondition(
                nameof(ProcessedAdvertisementDataModel.PriceAmount),
                ScanOperator.LessThanOrEqual,
                filter.MaxPrice.Value);
        }
        if (filter.MinRooms?.Value > 0)
        {
            scanFilter.AddCondition(
                nameof(ProcessedAdvertisementDataModel.RoomsValue),
                ScanOperator.GreaterThanOrEqual,
                filter.MinRooms.Value);
        }
        if (filter.MaxRooms?.Value > 0)
        {
            scanFilter.AddCondition(
                nameof(ProcessedAdvertisementDataModel.RoomsValue),
                ScanOperator.LessThanOrEqual,
                filter.MaxRooms.Value);
        }

        return await GetWhereAsync(scanFilter);
    }
}