using Amazon.DynamoDBv2.DocumentModel;
using Flvt.Domain.Primitives.Responses;
using Flvt.Infrastructure.Data.DataModels;
using Flvt.Infrastructure.Data.DataModels.Batches;

namespace Flvt.Infrastructure.Data.Repositories;

internal class BatchRepository : Repository<BatchDataModel>
{
    public BatchRepository(
        DataContext context,
        DataModelService<BatchDataModel> dataModelService) : base(
        context,
        dataModelService)
    {
    }

    public async Task<Result<IEnumerable<BatchDataModel>>> GetFinishedBatchesAsync()
    {
        var filter = new ScanFilter();
        filter.AddCondition(nameof(BatchDataModel.IsFinished), ScanOperator.Equal, new DynamoDBBool(true));

        return await GetWhereAsync(filter);
    }


    public async Task<Result<IEnumerable<BatchDataModel>>> GetUnfinishedBatchesAsync()
    {
        var filter = new ScanFilter();
        filter.AddCondition(nameof(BatchDataModel.IsFinished), ScanOperator.Equal, new DynamoDBBool(false));

        return await GetWhereAsync(filter);
    }
}