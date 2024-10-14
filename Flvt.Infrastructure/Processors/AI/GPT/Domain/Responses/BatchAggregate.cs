using Flvt.Infrastructure.Processors.AI.GPT.Domain.Batches;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.DataModels.Batches;

namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Responses;

internal sealed record BatchAggregate(Batch GPTBatch, BatchDataModel DataBatch);