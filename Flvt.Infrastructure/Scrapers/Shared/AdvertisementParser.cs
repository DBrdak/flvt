using Flvt.Domain.Primitives.Subscribers.Filters;
using HtmlAgilityPack;

namespace Flvt.Infrastructure.Scrapers.Shared;

public abstract class AdvertisementParser
{
    private HtmlDocument? _document;
    protected HtmlDocument Document => _document ?? throw new NullReferenceException("HtmlDocument is not set.");

    public void SetHtmlDocument(HtmlDocument htmlDocument)
    {
        _document = htmlDocument;
    }

    protected abstract string GetBaseUrl();
    protected abstract string GetBaseQueryRelativeUrl();
    public abstract string ParseQueryUrl(Filter filter);
    public abstract string ParsePagedQueryUrl(string baseQueryUrl, int page);
    public abstract List<string> ParseAdvertisementsLinks();
    public abstract string? ParseDescription();
    public abstract (string? Amount, string? Currency) ParsePrice();
    public abstract string? ParseContactType();
    public abstract string? ParseLocation();
    public abstract string? ParseSpecificFloor();
    public abstract string? ParseTotalFloors();
    public abstract (string? Count, string? Unit) ParseRooms();
    public abstract (string? Value, string? Unit) ParseArea();
    public abstract string? ParseAddedAt();
    public abstract string? ParseUpdatedAt();

}