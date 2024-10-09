using Flvt.Domain.Primitives.Subscribers.Filters;
using Flvt.Infrastructure.Scrapers.Shared;

namespace Flvt.Infrastructure.Scrapers.Olx;

internal sealed class OlxParser : AdvertisementParser
{
    protected override string GetBaseUrl() => "https://www.olx.pl";

    protected override string GetBaseQueryRelativeUrl() => "nieruchomosci/mieszkania";

    public override string ParseQueryUrl(Filter filter)
    {
        return null;
    }

    public override string ParsePagedQueryUrl(string baseQueryUrl, int page)
    {
        return null;
    }

    public override List<string> ParseAdvertisementsLinks()
    {
        return null;
    }

    public override string? ParseDescription()
    {
        return null;
    }

    public override (string? Amount, string? Currency) ParsePrice()
    {
        return (null, null);
    }

    public override string? ParseContactType()
    {
        return null;
    }

    public override string? ParseLocation()
    {
        return null;
    }

    public string? ParseFloor()
    {
        return null;
    }

    public override string? ParseSpecificFloor()
    {
        return null;
    }

    public override string? ParseTotalFloors()
    {
        return null;
    }

    public override (string? Count, string? Unit) ParseRooms()
    {
        return (null, null);
    }

    public override (string? Value, string? Unit) ParseArea()
    {
        return (null, null);
    }

    public override string? ParseAddedAt()
    {
        return null;
    }

    public override string? ParseUpdatedAt()
    {
        return null;
    }
}