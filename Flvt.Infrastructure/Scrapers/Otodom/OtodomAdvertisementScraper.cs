using Flvt.Infrastructure.Scrapers.Shared.Scrapers;

namespace Flvt.Infrastructure.Scrapers.Otodom;

internal sealed class OtodomAdvertisementScraper : AdvertisementScraper
{
    public OtodomAdvertisementScraper() : base(new OtodomParser())
    { }
}