using Flvt.Application.Abstractions;
using Flvt.Domain.Advertisements;
using Flvt.Domain.Subscribers;

namespace Flvt.Infrastructure.Scrapers.Otodom;

internal sealed class OtodomScraper : WebScraper
{
    private const string baseUrl = "https://www.otodom.pl/pl/wyniki";

    public OtodomScraper(Filter filter) : base(filter)
    {
    }

    public override async Task<IEnumerable<ScrapedAdvertisement>> ScrapeAsync()
    {
        return null;
    }

    protected override void BuildQueryUrl()
    {
    }
}