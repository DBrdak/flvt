using Flvt.Domain.Subscribers;
using Flvt.Infrastructure.Scrapers.Shared;

namespace Flvt.Infrastructure.Scrapers.Olx;

internal sealed class OlxScraper : AdvertisementScraper
{
    public OlxScraper(Filter filter) : base(filter, new OlxParser())
    {
    }
}