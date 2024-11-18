using Flvt.Infrastructure.Scrapers.Shared.Scrapers;

namespace Flvt.Infrastructure.Scrapers.Domiporta;

internal class DomiportaAdvertisementScraper : AdvertisementScraper
{
    public DomiportaAdvertisementScraper() : base(new DomiportaParser())
    {
    }
}