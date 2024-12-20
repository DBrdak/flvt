﻿using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure.Scrapers.Shared.Helpers;
using HtmlAgilityPack;

namespace Flvt.Infrastructure.Scrapers.Shared.Parsers;

internal abstract class AdvertisementParser
{
    private HtmlDocument? _document;
    protected HtmlDocument Document =>
        _document ?? throw new NullReferenceException("HtmlDocument is not set.");

    public void SetHtmlDocument(HtmlDocument htmlDocument)
    {
        _document = htmlDocument;
    }

    protected abstract string GetAdvertisementNodeSelector();
    protected abstract string GetContentNodeSelector();
    protected abstract string GetBaseUrl();
    protected abstract string GetBaseQueryRelativeUrl();
    public abstract string ParseQueryUrl(ScrapingFilter filter);
    public abstract string ParsePagedQueryUrl(string baseQueryUrl, int page);
    public abstract List<string> ParseAdvertisementsLinks();
    public abstract ScrapedAdContent ParseContent();
    public abstract IEnumerable<string> ParsePhotos();
    public abstract bool IsRateLimitExceeded(HtmlDocument htmlDocument);
    public abstract bool IsOutdatedAdvertisement();
}