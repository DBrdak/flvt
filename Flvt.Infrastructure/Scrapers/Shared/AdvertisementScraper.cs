﻿using System.Net;
using Flvt.Domain.Primitives.Subscribers.Filters;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure.Scrapers.Olx;
using HtmlAgilityPack;
using Serilog;

namespace Flvt.Infrastructure.Scrapers.Shared;

internal abstract class AdvertisementScraper
{
    public int SuccessfullyScrapedLinks;
    public int SuccessfullyScrapedAds;
    public int UnsuccessfullyScrapedAds;

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
                "Exception occured when trying to scrape advertisement link: {error}", e);
        }

        try
        {
            await ScrapeAdvertisementsAsync();
        }
        catch (Exception e)
        {
            Log.Logger.Error(
                "Exception occured when trying to scrape advertisement AdContent adContent: {error}", e);
        }

        SuccessfullyScrapedAds = _advertisements.Count;
        SuccessfullyScrapedLinks = _advertisementsLinks.Count;

        return _advertisements;
    }

    private async Task ScrapeAdvertisementsAsync()
    {
        foreach (var link in _advertisementsLinks)
        {
            try
            {
                var htmlDoc = await _web.LoadFromWebAsync(link);
                _advertisementParser.SetHtmlDocument(htmlDoc);

                var content = _advertisementParser.ParseContent();
                var photos = _advertisementParser.ParsePhotos();

                _advertisements.Add(new(link, content, photos));
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

                var htmlDoc = await _web.LoadFromWebAsync(pageUrl);

                _advertisementParser.SetHtmlDocument(htmlDoc);

                var links = _advertisementParser.ParseAdvertisementsLinks();

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
}