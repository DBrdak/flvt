namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.DataModels.Batches;

internal sealed record BatchDataModel(string BatchId, IEnumerable<string> ProcessingAdvertisementsLinks);