using Flvt.Infrastructure.Scrapers.Shared;

namespace Flvt.Infrastructure.Scrapers.Otodom;

internal sealed class OtodomLinkScraper : AdvertisementLinkScraper
{
    public OtodomLinkScraper(ScrapingFilter filter) : base(filter, new OtodomParser())
    {
    }
}