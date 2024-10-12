namespace Flvt.Domain.ScrapedAdvertisements;

public sealed class ScrapedAdvertisement
{
    public string Link { get; init; }
    public ScrapedContent Content { get; init; }
    public IEnumerable<string> Photos { get; init; }
    public bool IsProcessed { get; private set; }

    public ScrapedAdvertisement(
        string link,
        ScrapedContent content,
        IEnumerable<string> photos,
        bool isProcessed = false)
    {
        Link = link;
        Photos = photos;
        Content = content;
        IsProcessed = isProcessed;
    }

    private void Process() => IsProcessed = true;
}