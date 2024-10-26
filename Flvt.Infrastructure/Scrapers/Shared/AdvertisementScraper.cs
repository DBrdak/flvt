using Flvt.Domain.Filters;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure.Monitoring;
using Flvt.Infrastructure.Utlis.Extensions;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Serilog;

namespace Flvt.Infrastructure.Scrapers.Shared;

internal abstract class AdvertisementScraper
{
    public int SuccessfullyScrapedLinks;
    public int SuccessfullyScrapedAds;
    public Filter Filter => _filter;
    private const int advertisementChunkSize = 250;

    private readonly Filter _filter;
    private readonly HtmlWeb _web;
    private readonly AdvertisementParser _advertisementParser;
    private readonly HashSet<string> _advertisementsLinks = [];
    private readonly List<ScrapedAdvertisement> _advertisements = [];
    private readonly ScrapingMonitor _monitor;

    protected AdvertisementScraper(
        Filter filter,
        AdvertisementParser advertisementParser)
    {
        _filter = filter;
        _advertisementParser = advertisementParser;
        _web = new HtmlWeb();
        _monitor = new ScrapingMonitor(this);
    }

    public async Task<IEnumerable<ScrapedAdvertisement>> ScrapeAsync()
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

        try
        {
            var scrapeTasks = _advertisementsLinks
                .Chunk(advertisementChunkSize)
                .Select(ScrapeAdvertisementsAsync);

            await Task.WhenAll(scrapeTasks);
        }
        catch (Exception e)
        {
            Log.Logger.Error(
                "Exception occured when trying to scrape advertisement AdContent adContent: {error}", e);
        }

        SuccessfullyScrapedAds = _advertisements.Count;
        SuccessfullyScrapedLinks = _advertisementsLinks.Count;
        await _monitor.DisposeAsync();

        return _advertisements;
    }

    private async Task ScrapeAdvertisementsAsync(IEnumerable<string> links)
    {
        foreach (var link in links)
        {
            try
            {
                var htmlDoc = await _web.SafelyLoadFromUrlAsync(link, _advertisementParser);
                _advertisementParser.SetHtmlDocument(htmlDoc);

                var content = _advertisementParser.ParseContent();
                var photos = _advertisementParser.ParsePhotos();

                _advertisements.Add(new(link, JsonConvert.SerializeObject(content), photos));
            }
            catch (Exception e)
            {
                Log.Logger.Error(
                    "Exception occured when trying to scrape advertisement: {error}", e);
            }
        }
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