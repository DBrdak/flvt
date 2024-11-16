using Flvt.Domain.Filters;
using Flvt.Infrastructure.Scrapers.Shared;

namespace Flvt.Infrastructure.Scrapers.Otodom;

internal sealed class OtodomAdvertisementScraper : AdvertisementScraper
{
    public OtodomAdvertisementScraper() : base(new OtodomParser())
    { }
}