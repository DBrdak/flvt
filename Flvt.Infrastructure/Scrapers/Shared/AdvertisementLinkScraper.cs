using Flvt.Domain.Photos;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure.Monitoring;
using Flvt.Infrastructure.Utlis.Extensions;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Serilog;

namespace Flvt.Infrastructure.Scrapers.Shared;

internal abstract class AdvertisementLinkScraper
{
    public int SuccessfullyScrapedLinks;
    public ScrapingFilter Filter => _filter;

    private readonly ScrapingFilter _filter;
    private readonly HtmlWeb _web;
    private readonly AdvertisementParser _advertisementParser;
    private readonly HashSet<string> _advertisementsLinks = [];
    private readonly LinksScrapingMonitor _monitor;

    protected AdvertisementLinkScraper(
        ScrapingFilter filter,
        AdvertisementParser advertisementParser)
    {
        _filter = filter;
        _advertisementParser = advertisementParser;
        _web = new HtmlWeb();
        _monitor = new LinksScrapingMonitor(this);
    }

    public async Task<IEnumerable<string>> ScrapeAsync()
    {
        try
        {
            await ScrapeAdvertisementsLinksAsync();
        }
        catch (Exception e)
        {
            Log.Logger.Error(
                "Exception occured when trying to scrape advertisement link: {error}", e);
        }

        SuccessfullyScrapedLinks = _advertisementsLinks.Count;
        await _monitor.DisposeAsync();

        return _advertisementsLinks;
    }


    private async Task ScrapeAdvertisementsLinksAsync()
    {
        var page = 1;
        var isValidPage = true;
        var queryUrl = _advertisementParser.ParseQueryUrl(_filter);
        var pageUrl = queryUrl;

        do
        {
            try
            {
                pageUrl = _advertisementParser.ParsePagedQueryUrl(queryUrl, page);

                var links = await ScrapeAdvertisementLinksFromPage(pageUrl);

                isValidPage = links.Select(_advertisementsLinks.Add).ToList().Any(x => x);

                page++;
            }
            catch (Exception e)
            {
                Log.Logger.Error(
                    "Exception occured when trying to scrape advertisement links, on page: {url} - {error}", pageUrl, e);
            }
        }
        while (isValidPage);
    }

    private async Task<IEnumerable<string>> ScrapeAdvertisementLinksFromPage(string pageUrl)
    {
        var htmlDoc = await _web.SafelyLoadFromUrlAsync(pageUrl, _advertisementParser);

        _advertisementParser.SetHtmlDocument(htmlDoc);

        return _advertisementParser.ParseAdvertisementsLinks();
    }
}