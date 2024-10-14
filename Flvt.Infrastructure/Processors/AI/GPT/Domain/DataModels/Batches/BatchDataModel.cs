namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.DataModels.Batches;

internal sealed record BatchDataModel(string Id, IEnumerable<string> ProcessingAdvertisementsLinks);