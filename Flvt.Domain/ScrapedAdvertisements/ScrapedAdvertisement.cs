using Flvt.Domain.Extensions;
using Flvt.Domain.Primitives.Advertisements;
using Flvt.Domain.Primitives.Money;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ScrapedAdvertisements.Errors;

namespace Flvt.Domain.ScrapedAdvertisements;

public sealed class ScrapedAdvertisement
{
    public string Link { get; init; }
    public string HTML { get; init; }
    public bool IsProcessed { get; private set; }

    public ScrapedAdvertisement(string link, string html, bool isProcessed = false)
    {
        Link = link;
        HTML = html;
        IsProcessed = isProcessed;
    }

    private void Process() => IsProcessed = true;
}