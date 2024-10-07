using Flvt.Domain.Subscribers;
using Flvt.Infrastructure.Scrapers.Shared;

namespace Flvt.Infrastructure.Scrapers.Morizon;

internal sealed class MorizonScraper : AdvertisementScraper
{
    public MorizonScraper(Filter filter) : base(filter, new MorizonParser())
    {
    }
}