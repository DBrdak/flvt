using Flvt.Infrastructure.Scrapers.Shared.Helpers;

namespace Flvt.Infrastructure.Scrapers.Domiporta;

internal sealed class DomiportaLatestAdvertisementHelper
{
    public long LastScrapedId { get; init; }

    public DomiportaLatestAdvertisementHelper(ScraperHelper helper)
    {
        LastScrapedId = long.Parse(helper.Value);
    }

    public DomiportaLatestAdvertisementHelper(long lastScrapedId)
    {
        LastScrapedId = lastScrapedId;
    }

    public ScraperHelper ToScraperHelper() =>
        new (nameof(DomiportaLatestAdvertisementHelper), LastScrapedId.ToString());
}