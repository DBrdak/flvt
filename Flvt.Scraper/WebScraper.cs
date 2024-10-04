using Flvt.Domain.Advertisements;

namespace Flvt.Scraper;

internal abstract class WebScraper
{
    protected readonly Filter Filter;
    protected readonly string BaseUrl;
    protected string QueryUrl;

    protected WebScraper(Filter filter, string baseUrl)
    {
        Filter = filter;
        BaseUrl = baseUrl;
        QueryUrl = baseUrl;
    }

    public abstract Task<IEnumerable<ScrapedAdvertisement>> ScrapeAsync();

    protected abstract void BuildQueryUrl();
}