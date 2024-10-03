using Flvt.Domain.Advertisements;

namespace Flvt.Scraper.Olx;

internal sealed class OlxScraper : WebScraper
{
    private const string baseUrl = "https://www.olx.pl/nieruchomosci/mieszkania";

    public OlxScraper(Filter filter) : base(filter, baseUrl)
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