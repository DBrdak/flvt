using Amazon.DynamoDBv2.DocumentModel;
using Flvt.Domain.Primitives.Responses;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.DataModels.Batches;

namespace Flvt.Infrastructure.Data.Repositories;

internal class BatchRepository : Repository<BatchDataModel>
{
    public BatchRepository(DataContext context) : base(context)
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
        filter.AddCondition(nameof(BatchDataModel.IsFinished), ScanOperator.Equal, new DynamoDBBool(true));

        return await GetWhereAsync(filter);
    }
}