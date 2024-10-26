namespace Flvt.Domain.ScrapedAdvertisements;

public sealed class ScrapedAdvertisement
{
    public string Link { get; init; }
    public string AdContent { get; init; }
    public IEnumerable<string> Photos { get; init; }
    public bool IsProcessed { get; private set; }
    public long? ProcessingStartedAt { get; private set; }

    public ScrapedAdvertisement(
        string link,
        string adContent,
        IEnumerable<string> photos,
        long? processingStartedAt = null,
        bool isProcessed = false)
    {
        Link = link;
        Photos = photos;
        ProcessingStartedAt = processingStartedAt;
        AdContent = adContent;
        IsProcessed = isProcessed;
    }

    public void Process() => IsProcessed = true;
    public void StartProcessing() =>
        ProcessingStartedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    public void ProcessFailed() => 
        ProcessingStartedAt = null;
}