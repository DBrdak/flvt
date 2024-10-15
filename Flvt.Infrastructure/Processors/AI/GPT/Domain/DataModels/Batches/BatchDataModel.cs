namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.DataModels.Batches;

internal sealed record BatchDataModel
{
    public string Id { get; init; }
    public IEnumerable<string> ProcessingAdvertisementsLinks { get; init; }
    public bool IsFinished { get; private set; }

    public BatchDataModel(string Id,
        IEnumerable<string> ProcessingAdvertisementsLinks,
        bool IsFinished = false)
    {
        this.Id = Id;
        this.ProcessingAdvertisementsLinks = ProcessingAdvertisementsLinks;
        this.IsFinished = IsFinished;
    }

    public void FinishBatch() => IsFinished = true;
}