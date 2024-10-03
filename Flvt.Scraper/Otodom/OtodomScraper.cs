using Flvt.Domain.Advertisements;

namespace Flvt.Scraper.Otodom;

internal sealed class OtodomScraper : WebScraper
{
    private const string baseUrl = "https://www.otodom.pl/pl/wyniki";

    public OtodomScraper(Filter filter) : base(filter, baseUrl)
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