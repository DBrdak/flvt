using Flvt.Domain.Extensions;
using Flvt.Domain.Primitives.Advertisements;
using Flvt.Domain.Primitives.Subscribers.Filters;
using Flvt.Infrastructure.Scrapers.Otodom;
using Flvt.Infrastructure.Scrapers.Shared;
using HtmlAgilityPack;

namespace Flvt.Infrastructure.Scrapers.Olx;

internal sealed class OlxParser : AdvertisementParser
{
    private const string advertisementNodeSelector = "//div[@class='css-u2ayx9']/a[@class='css-z3gu2d']";
    private const string titleNodeSelector = "//div[@class='css-1kc83jo']";
    private const string descriptionNodeSelector = "//div[@class='css-1m8mzwg']";
    private const string extraDescriptionNodeSelector = "//div[@class='css-rn93um']";
    private const string priceNodeSelector = "//h3[@class='css-90xrc0']";
    private const string locationNodeSelector = "//div[@class='css-13l8eec']/div";
    private const string locationCityClassName = "css-1cju8pu";
    private const string locationStateClassName = "css-b5m1rv";
    private const string attributesNodeSelector = "//p[@class='css-b5m1rv']";
    private const string imageNodeSelector = "//img[@class='css-1bmvjcs']";
    private readonly List<string> _attributes = [];
    private const string areaKeyword = $"powierzchnia";
    private const string roomsKeyword = "liczba pokoi";
    private const string floorKeyword = "poziom";
    private const string addedAtNodeSelector = "//span[@class='css-19yf5ek']";
    protected override string GetBaseUrl() => "https://www.olx.pl";

    protected override string GetBaseQueryRelativeUrl() => "nieruchomosci/mieszkania/wynajem";

    public override string ParseQueryUrl(Filter filter)
    {
        var location = filter.Location.City.ToLower().ReplacePolishCharacters();

        var minPrice = filter.MinPrice is null ? string.Empty : $"search[filter_float_price:from]={filter.MinPrice}";
        var maxPrice = filter.MaxPrice is null ? string.Empty : $"search[filter_float_price:to]={filter.MaxPrice}";

        var minArea = filter.MinArea is null ? string.Empty : $"search[filter_float_m:from]={filter.MinArea}";
        var maxArea = filter.MaxArea is null ? string.Empty : $"search[filter_float_m:to]={filter.MaxArea}";

        if (filter.MinRooms is null && filter.MaxRooms is null)
        {
            return $"{GetBaseUrl()}/{GetBaseQueryRelativeUrl()}/{location}/?{minPrice}{maxPrice}{minArea}{maxArea}";
        }

        List<string> rooms = [];

        for (var i = filter.MinRooms?.Value ?? 1; i <= 4 && i <= (filter.MaxRooms?.Value ?? 4); i++)
        {
            rooms.Add($"search[filter_enum_rooms][{i}]={i.ToStandarizedString()}");
        }

        return $"{GetBaseUrl()}/{GetBaseQueryRelativeUrl()}/{location}/?{minPrice}&{maxPrice}&{minArea}&{maxArea}&{rooms}";
    }

    public override string ParsePagedQueryUrl(string baseQueryUrl, int page) => $"{baseQueryUrl}&page={page}";

    public override List<string> ParseAdvertisementsLinks()
    {
        var advertisements = Document.DocumentNode.SelectNodes(advertisementNodeSelector).ToList();
        var relativeLinks = advertisements.Select(
            ad => ad.GetAttributeValue(
                "href",
                string.Empty))
            .Where(link => !link.ToLower().Contains("otodom"))
            .ToList();

        return relativeLinks.Select(link => string.Concat(GetBaseUrl(), link)).ToList();
    }

    public override string? ParseDescription() =>
        string.Join(" ",
                "Title:", Document.DocumentNode.SelectSingleNode(titleNodeSelector).InnerText,
                "Description:", Document.DocumentNode.SelectSingleNode(descriptionNodeSelector)
                    ?.InnerText,
                "Specification", Document.DocumentNode.SelectSingleNode(extraDescriptionNodeSelector)
                    ?.InnerText)
            .Trim() is var result && string.IsNullOrWhiteSpace(result) ? null : result;

    public override (string? Amount, string? Currency) ParsePrice()
    {
        var priceText = Document.DocumentNode.SelectSingleNode(priceNodeSelector)?.InnerText.Trim()
            .Replace(" ", "");
        var priceAmount = string.Join("", priceText?.Where(char.IsDigit) ?? "");
        var priceCurrency = string.Join("", priceText?.Where(char.IsLetter) ?? "");

        return (priceAmount, priceCurrency);
    }

    public override string? ParseContactType() => string.Empty;

    public override string? ParseLocation() =>
        string.Join(
            string.Empty,
            Document.DocumentNode.SelectSingleNode(locationNodeSelector)
                .ChildNodes.Where(
                    node => node.HasClass(locationCityClassName) || node.HasClass(locationStateClassName))
                .Select(node => node.InnerText.Trim()));

    private void ParseAttributes()
    {
        _attributes.AddRange(
            Document.DocumentNode.SelectNodes(attributesNodeSelector).Select(node => node.InnerText.ToLower().Trim()));
    }

    public override string? ParseSpecificFloor()
    {
        if (!_attributes.Any())
        {
            ParseAttributes();
        }

        return _attributes.FirstOrDefault(attr => attr.Contains(floorKeyword))?.Trim() is var floor &&
               floor?.ToLower() == "parter" ?
            "0" :
            floor;
    }

    public override string? ParseTotalFloors() => string.Empty;

    public override (string? Count, string? Unit) ParseRooms()
    {
        if (!_attributes.Any())
        {
            ParseAttributes();
        }

        var rooms =
            _attributes.FirstOrDefault(attr => attr.Contains(roomsKeyword))?.Split(" ")[1] is var roomCount &&
            roomCount?.ToLower() == "kawalerka" ?
                "1" :
                roomCount;

        return (rooms, string.Empty);
    }

    public override (string? Value, string? Unit) ParseArea()
    {
        if (!_attributes.Any())
        {
            ParseAttributes();
        }

        var area = _attributes.FirstOrDefault(attr => attr.Contains(areaKeyword))?.Trim();
        var value = area?.Split(" ")[1];
        var unit = area?.Split(" ")[2];

        return (value, unit);
    }

    public override string? ParseAddedAt() =>
        Document.DocumentNode.SelectSingleNode(addedAtNodeSelector).InnerText.Trim();

    public override string? ParseUpdatedAt() => string.Empty;

    public override IEnumerable<string>? ParsePhotos()
    {
        var a = "";

        return Document.DocumentNode.SelectNodes(imageNodeSelector)
            .Select(
                node =>
                {
                    node.GetAttributeValue("src", a);
                    return a;
                });
    }
}