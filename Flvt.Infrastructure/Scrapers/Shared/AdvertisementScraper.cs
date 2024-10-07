using System.Globalization;
using Flvt.Domain.Advertisements;
using Flvt.Domain.Subscribers;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Serilog;

namespace Flvt.Infrastructure.Scrapers.Shared;

internal abstract class AdvertisementScraper
{
    private readonly Filter _filter;
    private readonly HtmlWeb _web;
    private readonly AdvertisementParser _advertisementParser;
    private readonly HashSet<string> _advertisementsLinks = [];
    private readonly List<ScrapedAdvertisement> _advertisements = [];

    protected AdvertisementScraper(Filter filter, AdvertisementParser advertisementParser)
    {
        _filter = filter;
        _advertisementParser = advertisementParser;
        _web = new HtmlWeb();
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
                $"Exception occured when trying to scrape advertisement links: {JsonConvert.SerializeObject(e)}");
        }

        try
        {
            await ScrapeAdvertisementsContentAsync();
        }
        catch (Exception e)
        {
            Log.Logger.Error(
                $"Exception occured when trying to scrape advertisement content: {JsonConvert.SerializeObject(e)}");
        }

        return _advertisements;
    }

    private async Task ScrapeAdvertisementsContentAsync()
    {
        var tasks = new List<Task>();

        foreach (var advertisementLink in _advertisementsLinks)
        {
            //var task = ScrapeAdvertisementContentAsync(advertisementLink);
            //tasks.Add(task);
            await ScrapeAdvertisementContentAsync(advertisementLink);
        }

        await Task.WhenAll(tasks);
    }

    private async Task ScrapeAdvertisementContentAsync(string advertisementLink)
    {
        var htmlDoc = await _web.LoadFromWebAsync(advertisementLink);
        _advertisementParser.SetHtmlDocument(htmlDoc);

        var location = _advertisementParser.ParseLocation();
        var description = _advertisementParser.ParseDescription();
        var contactType = _advertisementParser.ParseContactType(); // TODO problem
        var price = _advertisementParser.ParsePrice(); // TODO problem
        var floor = _advertisementParser.ParseFloor(); // TODO problem
        var area = _advertisementParser.ParseArea(); // TODO problem
        var rooms = _advertisementParser.ParseRooms(); // TODO problem
        var addedAt = _advertisementParser.ParseAddedAt();
        var updatedAt = _advertisementParser.ParseUpdatedAt();

        var createResult = ScrapedAdvertisement.CreateFromScrapedContent(
            advertisementLink,
            location,
            description,
            contactType,
            price.Amount,
            price.Currency,
            rooms.Count,
            rooms.Unit,
            area.Value,
            area.Unit,
            floor,
            addedAt,
            updatedAt);

        if (createResult.IsSuccess)
        {
            _advertisements.Add(createResult.Value);
            return;
        }

        Log.Error($"Failed to create ScrapedAdvertisement, error: {createResult.Error}. Advertisement link: {advertisementLink}");
    }

    private async Task ScrapeAdvertisementsLinksAsync()
    {
        var page = 1;
        bool isValidPage;
        var queryUrl = _advertisementParser.ParseQueryUrl(_filter);

        do
        {
            var pageUrl = _advertisementParser.ParsePagedQueryUrl(queryUrl, page);
            var htmlDoc = await _web.LoadFromWebAsync(pageUrl);
            _advertisementParser.SetHtmlDocument(htmlDoc);

            var links = _advertisementParser.ParseAdvertisementsLinks();

            isValidPage = links.Select(_advertisementsLinks.Add).ToList().Any(x => x);

            page++;
        }
        while (isValidPage);
    }
}