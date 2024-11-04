using Flvt.Infrastructure.Data.DataModels.Batches;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Batches;

namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Responses;

internal sealed record BatchAggregate(Batch GPTBatch, BatchDataModel DataBatch);