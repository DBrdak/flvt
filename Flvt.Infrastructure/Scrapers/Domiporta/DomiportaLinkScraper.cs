using Flvt.Infrastructure.Scrapers.Shared.Helpers;
using Flvt.Infrastructure.Scrapers.Shared.Scrapers;

namespace Flvt.Infrastructure.Scrapers.Domiporta;

internal sealed class DomiportaLinkScraper : AdvertisementLinkScraper
{
    private readonly DomiportaLatestAdvertisementHelper _latestAdvertisementHelper;

    public DomiportaLinkScraper(ScrapingFilter filter, DomiportaLatestAdvertisementHelper latestAdvertisementHelper) 
        : base(filter, new DomiportaParser())
    {
        _latestAdvertisementHelper = latestAdvertisementHelper;
    }

    protected override List<string> ValidateLinks(List<string> links)
    {
        var validLinks = links
            .Where(link => GetIdFromLink(link) > _latestAdvertisementHelper.LastScrapedIdIn(Filter.City))
            .ToList();

        _latestAdvertisementHelper.CurrentlyScrapedIds.AddRange(validLinks.Select(GetIdFromLink));

        return validLinks;
    }

    private long GetIdFromLink(string link) => 
        long.Parse(link.Split("/").Last());
}