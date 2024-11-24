using Flvt.Infrastructure.Scrapers.Shared.Helpers;

namespace Flvt.Infrastructure.Scrapers.Domiporta;

internal sealed class DomiportaLatestAdvertisementHelper
{
    public long LastScrapedId { get; private set; }
    public List<long> CurrentlyScrapedIds { get; private set; } = [];

    public DomiportaLatestAdvertisementHelper(ScraperHelper helper)
    {
        LastScrapedId = long.Parse(helper.Value);
    }

    public DomiportaLatestAdvertisementHelper(long lastScrapedId)
    {
        LastScrapedId = lastScrapedId;
    }

    public ScraperHelper ToScraperHelper()
    {
        UpdateLastScrapedId();

        return new ScraperHelper(nameof(DomiportaLatestAdvertisementHelper), LastScrapedId.ToString());
    }

    private void UpdateLastScrapedId()
    {
        if (!CurrentlyScrapedIds.Any())
        {
            return;
        }

        LastScrapedId = CurrentlyScrapedIds.Max() is var maxId && maxId > LastScrapedId ?
            maxId :
            LastScrapedId;
    }
}