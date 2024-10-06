using Flvt.Application.Abstractions;
using Flvt.Domain.Advertisements;
using Flvt.Domain.Subscribers;
using HtmlAgilityPack;

namespace Flvt.Infrastructure.Scrapers;

internal abstract class WebScraper
{
    protected readonly Filter Filter;
    protected string QueryUrl;
    protected readonly HtmlWeb Web;

    protected WebScraper(Filter filter)
    {
        Filter = filter;
        QueryUrl = string.Empty;
        Web = new HtmlWeb();
    }

    public abstract Task<IEnumerable<ScrapedAdvertisement>> ScrapeAsync();

    protected abstract void BuildQueryUrl();
}