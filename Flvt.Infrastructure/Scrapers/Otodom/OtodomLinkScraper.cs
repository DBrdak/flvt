using Flvt.Infrastructure.Scrapers.Shared.Helpers;
using Flvt.Infrastructure.Scrapers.Shared.Scrapers;

namespace Flvt.Infrastructure.Scrapers.Otodom;

internal sealed class OtodomLinkScraper : AdvertisementLinkScraper
{
    public OtodomLinkScraper(ScrapingFilter filter) : base(filter, new OtodomParser())
    {
    }
}