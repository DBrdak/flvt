using Flvt.Domain.Primitives.Subscribers.Filters;
using Flvt.Domain.ScrapedAdvertisements;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Serilog;

namespace Flvt.Infrastructure.Scrapers.Shared;

internal abstract class AdvertisementScraper
{
    private readonly Filter _filter;
    private readonly HtmlWeb _web;
    private readonly AdvertisementParser _advertisementParser;
    private HashSet<string> _advertisementsLinks = [];
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
                "Exception occured when trying to scrape advertisement link: {error}", e);
        }

        try
        {
            await ScrapeAdvertisementsContentAsync();
        }
        catch (Exception e)
        {
            Log.Logger.Error(
                "Exception occured when trying to scrape advertisement content, error: {error}", e);
        }

        return _advertisements;
    }

    private async Task ScrapeAdvertisementsContentAsync()
    {
        var tasks = new List<Task>();

        foreach (var advertisementLink in _advertisementsLinks)
        {
            var task = ScrapeAdvertisementContentAsync(advertisementLink);
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
    }

    private async Task ScrapeAdvertisementContentAsync(string advertisementLink)
    {
        var htmlDoc = await _web.LoadFromWebAsync(advertisementLink);
        _advertisementParser.SetHtmlDocument(htmlDoc);

        var location = _advertisementParser.ParseLocation();
        var description = _advertisementParser.ParseDescription();
        var contactType = _advertisementParser.ParseContactType();
        var price = _advertisementParser.ParsePrice();
        var specificFloor = _advertisementParser.ParseSpecificFloor();
        var totalFloors = _advertisementParser.ParseTotalFloors();
        var area = _advertisementParser.ParseArea();
        var rooms = _advertisementParser.ParseRooms();
        var addedAt = _advertisementParser.ParseAddedAt();
        var updatedAt = _advertisementParser.ParseUpdatedAt();
        var photos = _advertisementParser.ParsePhotos();

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
            specificFloor, 
            totalFloors,
            addedAt,
            updatedAt,
            photos);

        if (createResult.IsSuccess)
        {
            _advertisements.Add(createResult.Value);
            return;
        }

        Log.Warning(
            "Failed to create ScrapedAdvertisement, error: {error}. Advertisement link: {link}",
            createResult.Error,
            advertisementLink);
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