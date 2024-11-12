using Flvt.Domain.Filters;
using Flvt.Domain.Photos;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure.Monitoring;
using Flvt.Infrastructure.Utlis.Extensions;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Serilog;

namespace Flvt.Infrastructure.Scrapers.Shared;

internal abstract class AdvertisementScraper
{
    public int SuccessfullyScrapedAds;
    private const int advertisementChunkSize = 250;

    private readonly HtmlWeb _web;
    private readonly AdvertisementParser _advertisementParser;
    private readonly List<ScrapedAdvertisement> _advertisements = [];
    private readonly List<AdvertisementPhotos> _photos = [];
    private readonly AdsScrapingMonitor _monitor;

    protected AdvertisementScraper(AdvertisementParser advertisementParser)
    {
        _advertisementParser = advertisementParser;
        _web = new HtmlWeb();
        _monitor = new AdsScrapingMonitor(this);
    }

    public async Task<AdvertisementsScrapeResult> ScrapeAsync(IEnumerable<string> links)
    {
        try
        {
            var scrapeTasks = links
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
        await _monitor.DisposeAsync();

        return new (_advertisements, _photos);
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
                var photos = _advertisementParser.ParsePhotos().ToList();

                _advertisements.Add(new(link, JsonConvert.SerializeObject(content)));
                _photos.Add(new AdvertisementPhotos(link, photos));
            }
            catch (Exception e)
            {
                Log.Logger.Error(
                    "Exception occured when trying to scrape advertisement: {error}", e);
            }
        }
    }
}