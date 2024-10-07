using Flvt.Domain.Extensions;
using Flvt.Domain.Subscribers;
using Flvt.Infrastructure.Scrapers.Shared;
using HtmlAgilityPack;

namespace Flvt.Infrastructure.Scrapers.Morizon;

internal class MorizonParser : AdvertisementParser
{
    private const string advertisementNodeSelector = "//a[contains(@class, 'q-Oe37')]";
    private const string titleNodeSelector = "//div[contains(@class, 'SAxzxG')]";
    private const string descriptionNodeSelector = "//div[contains(@class, '_0A-7u8')]";
    private const string extraDescriptionNodeSelector = "//div[contains(@class, 'SQVgAz')]";
    private const string priceNodeSelector = "//span[contains(@class, 'LphL0t')]";
    private const string contactTypeNodeSelector = "//div[contains(@class, 'lf4Mw8')]";
    private const string locationNodeSelector = "//h2[contains(@class, 'y1mnyH')]";
    private const string floorRoomsAreaNodeSelector = "//div[contains(@class, 'Ca6gX5')]";
    private const string addedAtNodeSelector = "//div[contains(@class, 'vZJg9t') and .//span[text()='Data dodania']]//div[@data-cy='itemValue']";
    private const string updatedAtNodeSelector = "//div[contains(@class, 'vZJg9t') and .//span[text()='Aktualizacja']]//div[@data-cy='itemValue']";
    private const char roomsFloorAreaSeparator = '•';
    private const int roomsIndex = 1;
    private const int areaIndex = 2;
    private const int floorIndex = 3;
    private string? _floorRoomsArea = string.Empty;

    protected override string GetBaseUrl() => "https://www.morizon.pl";

    protected override string GetBaseQueryRelativeUrl() => "do-wynajecia/mieszkania";

    public override string ParseQueryUrl(Filter filter)
    {
        var location = filter.Location.ToLower().ReplacePolishCharacters();

        var minPrice = filter.MinPrice is null ? string.Empty : $"ps[price_from]={filter.MinPrice}";
        var maxPrice = filter.MaxPrice is null ? string.Empty : $"ps[price_to]={filter.MaxPrice}";

        var minArea = filter.MinArea is null ? string.Empty : $"ps[living_area_from]={filter.MinArea}";
        var maxArea = filter.MaxArea is null ? string.Empty : $"ps[living_area_to]={filter.MaxArea}";

        var minRooms = filter.MinRooms is null ? string.Empty : $"ps[number_of_rooms_from]={filter.MinRooms}";
        var maxRooms = filter.MaxRooms is null ? string.Empty : $"ps[number_of_rooms_to]={filter.MaxRooms}";

        return $"{GetBaseUrl()}/{GetBaseQueryRelativeUrl()}/{location}/?{minPrice}&{maxPrice}&{minArea}&{maxArea}&{minRooms}&{maxRooms}";
    }

    public override string ParsePagedQueryUrl(string baseQueryUrl, int page) => $"{baseQueryUrl}&page={page}";

    public override List<string> ParseAdvertisementsLinks()
    {
        var advertisements = Document.DocumentNode.SelectNodes(advertisementNodeSelector).ToList();

        return advertisements.Select(
                ad => string.Concat(
                    GetBaseUrl(),
                    ad.GetAttributeValue(
                        "href",
                        string.Empty)))
            .ToList();
    }

    public override string? ParseDescription() =>
        string.Join(" ",
                "Title:", Document.DocumentNode.SelectSingleNode(titleNodeSelector),
                "Description", Document.DocumentNode.SelectSingleNode(descriptionNodeSelector)
                    ?.InnerText,
                "Specification:", string.Join(
                    " ",
                    Document.DocumentNode.SelectNodes(extraDescriptionNodeSelector)
                        .Select(node => node.InnerText)))
            .Trim() is var result && string.IsNullOrWhiteSpace(result) ? null : result;

    public override (string? Amount, string? Currency) ParsePrice()
    {
        var price = Document.DocumentNode.SelectSingleNode(priceNodeSelector)
            ?.InnerText.Trim();
        var priceAmount = string.Join("", price?.Where(char.IsDigit) ?? "");
        var priceCurrency = string.Join("", price?.Where(char.IsLetter) ?? "");

       return (priceAmount, priceCurrency);
    }

    public override string ParseContactType() =>
        Document.DocumentNode.SelectSingleNode(contactTypeNodeSelector)
            ?.InnerText.Trim() ?? "agencja nieruchomości";

    public override string? ParseLocation() =>
        Document.DocumentNode.SelectSingleNode(locationNodeSelector)
            ?.InnerText.Trim();

    public override string? ParseFloor()
    {
        PrepareFloorRoomsArea();

        return _floorRoomsArea?.Split(roomsFloorAreaSeparator).ElementAtOrDefault(floorIndex)?.Trim();
    }

    public override (string? Count, string? Unit) ParseRooms()
    {
        var roomsCount = _floorRoomsArea?.Split(roomsFloorAreaSeparator)
            .ElementAtOrDefault(roomsIndex)
            ?.Trim()
            .Split(" ")
            .ElementAtOrDefault(0);
        var roomsUnit = _floorRoomsArea?.Split(roomsFloorAreaSeparator)
            .ElementAtOrDefault(roomsIndex)
            ?.Trim()
            .Split(" ")
            .ElementAtOrDefault(1);

        return (roomsCount, roomsUnit);
    }

    public override (string? Value, string? Unit) ParseArea()
    {
        
        var areaValue = _floorRoomsArea?.Split(roomsFloorAreaSeparator)
            .ElementAtOrDefault(areaIndex)
            ?.Trim()
            .Split(" ")
            .ElementAtOrDefault(0);
        var areaUnit = _floorRoomsArea?.Split(roomsFloorAreaSeparator)
            .ElementAtOrDefault(areaIndex)
            ?.Trim()
            .Split(" ")
            .ElementAtOrDefault(1);

        return (areaValue, areaUnit);
    }

    public override string? ParseAddedAt() =>
        Document.DocumentNode.SelectSingleNode(addedAtNodeSelector)
            ?.InnerText.Trim();

    public override string? ParseUpdatedAt() =>
        Document.DocumentNode.SelectSingleNode(updatedAtNodeSelector)
            ?.InnerText.Trim();

    private void PrepareFloorRoomsArea()
    {
        if (_floorRoomsArea != string.Empty)
        {
            return;
        }

        _floorRoomsArea = Document.DocumentNode.SelectSingleNode(floorRoomsAreaNodeSelector)?.InnerText;
    }
}