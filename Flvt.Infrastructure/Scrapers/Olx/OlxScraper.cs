using Flvt.Application.Abstractions;
using Flvt.Domain.Advertisements;
using Flvt.Domain.Subscribers;

namespace Flvt.Infrastructure.Scrapers.Olx;

internal sealed class OlxScraper : WebScraper
{
    private const string baseUrl = "https://www.olx.pl/nieruchomosci/mieszkania";

    public OlxScraper(Filter filter) : base(filter)
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