﻿using System.Text.RegularExpressions;
using Flvt.Domain.Extensions;
using Flvt.Domain.Subscribers;
using Flvt.Infrastructure.Scrapers.Shared;
using HtmlAgilityPack;

namespace Flvt.Infrastructure.Scrapers.Otodom;

internal sealed class OtodomParser : AdvertisementParser
{
    private const string advertisementNodeSelector = "//a[contains(@class, 'css-16vl3c1 e17g0c820')]";
    private const string titleNodeSelector = "//div[contains(@class, 'css-1y950rh ehdsj770')]";
    private const string descriptionNodeSelector = "//div[contains(@class, 'css-v0orps e14akrx11')]";
    private const string extraDescriptionNodeSelector = "//div[contains(@class, 'css-1xbf5wd e1qhas4i0')]";
    private const string priceNodeSelector = "//strong[@class='css-1o51x5a e1w5xgvx1']";
    private const string contactTypeNodeSelector = "//div[contains(@class, 'css-t7cajz e1qhas4i1')]";
    private const string contactTypeKeyword = "typ ogłoszeniodawcy";
    private const string locationNodeSelector = "//a[contains(@class, 'css-1jjm9oe e42rcgs1')]";
    private const string roomsAreaNodeSelector = "//div[@class='css-1ftqasz']";
    private const string areaPattern = "m²";
    private const string roomsKeyword = "pok";
    private const string floorNodeSelector = "//div[contains(@class, 'css-t7cajz e1qhas4i1')]";
    private const string addedAtNodeSelector = "//div[@class='css-1821gv5 e82kd4s1']/p[contains(text(), 'Dodano:')]";
    private const string updatedAtNodeSelector = "//div[@class='css-1821gv5 e82kd4s1']/p[contains(text(), 'Aktualizacja:')]";
    private readonly List<HtmlNode> _roomsAreaNodes = []; 

    protected override string GetBaseUrl() => "https://www.otodom.pl";

    protected override string GetBaseQueryRelativeUrl() => "pl/wyniki/wynajem/mieszkanie";

    public override string ParseQueryUrl(Filter filter)
    {
        var location = filter.OtodomLocation()?.ToLower().ReplacePolishCharacters();

        var minPrice = filter.MinPrice is null ? string.Empty : $"priceMin={filter.MinPrice}";
        var maxPrice = filter.MaxPrice is null ? string.Empty : $"priceMax={filter.MaxPrice}";

        var minArea = filter.MinArea is null ? string.Empty : $"areaMin={filter.MinArea}";
        var maxArea = filter.MaxArea is null ? string.Empty : $"areaMax={filter.MaxArea}";

        if (filter.MinRooms is null && filter.MaxRooms is null)
        {
            return $"{GetBaseUrl()}/{GetBaseQueryRelativeUrl()}/{location}/?{minPrice}{maxPrice}{minArea}{maxArea}";
        }

        List<string> roomsValues = [];
        var sixRoomsValue = "SIX_OR_MORE";

        for (var i = filter.MinRooms ?? 1; i <= 6 && i <= (filter.MaxRooms ?? 6); i++)
        {
            roomsValues.Add(i == 6 ? sixRoomsValue : i.ToStandarizedString());
        }

        var rooms = $"roomsNumber=[{string.Join(',', roomsValues)}]";

        return $"{GetBaseUrl()}/{GetBaseQueryRelativeUrl()}/{location}/?{minPrice}&{maxPrice}&{minArea}&{maxArea}&{rooms}";
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
                "Description:", Document.DocumentNode.SelectSingleNode(descriptionNodeSelector)
                    ?.InnerText,
                "Specification", Document.DocumentNode.SelectSingleNode(extraDescriptionNodeSelector)
                    ?.InnerText)
            .Trim() is var result && string.IsNullOrWhiteSpace(result) ? null : result;

    public override (string? Amount, string? Currency) ParsePrice()
    {
        var price = Document.DocumentNode.SelectSingleNode(priceNodeSelector)
            ?.InnerText.Trim();
        var priceAmount = string.Join("", price?.Where(char.IsDigit) ?? "");
        var priceCurrency = string.Join("", price?.Where(char.IsLetter) ?? "");

        return (priceAmount, priceCurrency);
    }

    public override string? ParseContactType() =>
        Document.DocumentNode.SelectNodes(contactTypeNodeSelector)
            .FirstOrDefault(node => node.InnerText.ToLower().Contains(contactTypeKeyword))
            ?.ChildNodes.ElementAtOrDefault(1)
            ?.InnerText.Trim();

    public override string? ParseLocation() =>
        Document.DocumentNode.SelectSingleNode(locationNodeSelector)
            ?.InnerText.Trim();

    public override string? ParseFloor() =>
        Document.DocumentNode.SelectNodes(floorNodeSelector)
            .FirstOrDefault(node => node.InnerText.ToLower().Contains("piętro"))
            ?.ChildNodes.ElementAtOrDefault(1)
            ?.InnerText.Trim();

    public override (string? Count, string? Unit) ParseRooms()
    {
        if (_roomsAreaNodes.Count == 0)
        {
            PrepareRoomsArea();
        }

        var rooms = _roomsAreaNodes.FirstOrDefault(node => node.InnerText.Contains(roomsKeyword))
            ?.InnerText.Trim();

        if (rooms is null)
        {
            return (null, null);
        }

        var roomsCount = rooms.Split(" ").ElementAtOrDefault(0);
        var roomsUnit = rooms.Split(" ").ElementAtOrDefault(1);

        return (roomsCount, roomsUnit);
    }

    public override (string? Value, string? Unit) ParseArea()
    {
        if (_roomsAreaNodes.Count == 0)
        {
            PrepareRoomsArea();
        }

        var area = _roomsAreaNodes.FirstOrDefault(node => node.InnerText.Contains(areaPattern))
            ?.InnerText.Trim();

        if (area is null)
        {
            return (null, null);
        }

        var areaValue = area[..area.IndexOf('m')];
        var areaUnit = area.Substring(area.IndexOf('m'), 2);

        return (areaValue, areaUnit);
    }

    public override string? ParseAddedAt() =>
        Document.DocumentNode.SelectSingleNode(addedAtNodeSelector)
            ?.InnerText.Trim().Split(" ").ElementAtOrDefault(1);

    public override string? ParseUpdatedAt() =>
        Document.DocumentNode.SelectSingleNode(updatedAtNodeSelector)
            ?.InnerText.Trim().Split(" ").ElementAtOrDefault(1);

    private void PrepareRoomsArea() =>
        _roomsAreaNodes.AddRange(Document.DocumentNode.SelectNodes(roomsAreaNodeSelector));
}