using Flvt.Infrastructure.Data;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.DataModels.Batches;

namespace Flvt.Infrastructure.Data.Repositories;

internal class BatchRepository : Repository<BatchDataModel>
{
    public BatchRepository(DataContext context) : base(context)
    {
    }
}