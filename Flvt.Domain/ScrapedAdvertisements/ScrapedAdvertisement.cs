namespace Flvt.Domain.ScrapedAdvertisements;

public sealed class ScrapedAdvertisement
{
    public string Link { get; init; }
    public ScrapedAdContent AdContent { get; init; }
    public IEnumerable<string> Photos { get; init; }
    public bool IsProcessed { get; private set; }

    public ScrapedAdvertisement(
        string link,
        ScrapedAdContent adContent,
        IEnumerable<string> photos,
        bool isProcessed = false)
    {
        Link = link;
        Photos = photos;
        AdContent = adContent;
        IsProcessed = isProcessed;
    }

    private void Process() => IsProcessed = true;
}