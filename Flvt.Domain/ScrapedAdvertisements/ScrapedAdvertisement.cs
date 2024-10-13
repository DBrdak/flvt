namespace Flvt.Domain.ScrapedAdvertisements;

public sealed class ScrapedAdvertisement
{
    public string Link { get; init; }
    public ScrapedAdContent AdContent { get; init; }
    public IEnumerable<string> Photos { get; init; }
    public bool IsProcessed { get; private set; }
    public long? ProcessingStartedAt { get; private set; }
    public string? BatchId { get; private set; }

    public ScrapedAdvertisement(
        string link,
        ScrapedAdContent adContent,
        IEnumerable<string> photos,
        long? processingStartedAt = null,
        string? batchId = null,
        bool isProcessed = false)
    {
        Link = link;
        Photos = photos;
        ProcessingStartedAt = processingStartedAt;
        BatchId = batchId;
        AdContent = adContent;
        IsProcessed = isProcessed;
    }

    private void StartProcessing() => ProcessingStartedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    public void Process() => IsProcessed = true;
    public void AddToBatch(string batchId)
    {
        BatchId = batchId;
        StartProcessing();
    }
}