using Flvt.Domain.Extensions;
using Flvt.Domain.Filters;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure.Scrapers.Shared;
using HtmlAgilityPack;

namespace Flvt.Infrastructure.Scrapers.Otodom;

internal sealed class OtodomParser : AdvertisementParser
{
    private OtodomAdAdContent? _content;

    protected override string GetAdvertisementNodeSelector() => "//a[@data-cy='listing-item-link']";

    protected override string GetContentNodeSelector() => "//script[@id='__NEXT_DATA__']";

    protected override string GetBaseUrl() => "https://www.otodom.pl";

    protected override string GetBaseQueryRelativeUrl() => "pl/wyniki/wynajem/mieszkanie";

    public override string ParseQueryUrl(Filter filter)
    {
        var location = filter.OtodomLocation()?.ToLower().ReplacePolishCharacters();
        var createdInLast24H = filter.OnlyLast24H ? "daysSinceCreated=1" : string.Empty;

        var minPrice = filter.MinPrice is null ? string.Empty : $"priceMin={filter.MinPrice}";
        var maxPrice = filter.MaxPrice is null ? string.Empty : $"priceMax={filter.MaxPrice}";

        var minArea = filter.MinArea is null ? string.Empty : $"areaMin={filter.MinArea}";
        var maxArea = filter.MaxArea is null ? string.Empty : $"areaMax={filter.MaxArea}";

        if (filter.MinRooms is null && filter.MaxRooms is null)
        {
            return
                $"{GetBaseUrl()}/{GetBaseQueryRelativeUrl()}/{location}/?{minPrice}&{maxPrice}&{minArea}&{maxArea}&{createdInLast24H}";
        }

        List<string> roomsValues = [];
        var sixRoomsValue = "SIX_OR_MORE";

        for (var i = filter.MinRooms?.Value ?? 1; i <= 6 && i <= (filter.MaxRooms?.Value ?? 6); i++)
        {
            roomsValues.Add(i == 6 ? sixRoomsValue : i.ToStandarizedString());
        }

        var rooms = $"roomsNumber=[{string.Join(',', roomsValues)}]";

        return $"{GetBaseUrl()}/{GetBaseQueryRelativeUrl()}/{location}/?{minPrice}&{maxPrice}&{minArea}&{maxArea}&{rooms}&{createdInLast24H}";
    }

    public override string ParsePagedQueryUrl(string baseQueryUrl, int page) => $"{baseQueryUrl}&page={page}";

    public override List<string> ParseAdvertisementsLinks()
    {
        var advertisements = Document.DocumentNode.SelectNodes(GetAdvertisementNodeSelector())?.ToList();

        return advertisements?.Select(
                ad => string.Concat(
                    GetBaseUrl(),
                    ad.GetAttributeValue(
                        "href",
                        string.Empty)))
            .ToList() ?? [];
    }

    public override ScrapedAdContent ParseContent()
    {
        var nodeJson = Document.DocumentNode.SelectSingleNode(GetContentNodeSelector()).InnerHtml;

        _content = OtodomAdAdContent.FromJson(nodeJson);

        return _content;
    }

    public override IEnumerable<string> ParsePhotos() => _content?.Images.Select(image => image.Large) ?? [];

    public override bool IsRateLimitExceeded(HtmlDocument htmlDocument) =>
        htmlDocument
            .DocumentNode
            .SelectSingleNode("//title")
            .InnerText
            .ToLower() is var title &&
        title.Contains("error: the request could not be satisfied");

    public override bool IsOutdatedAdvertisement() =>
        Document
            .DocumentNode
            .SelectSingleNode("//div[@data-cy='expired-ad-alert']")
            is not null 
        ||
        Document
            .DocumentNode
            .SelectSingleNode("//div[@data-cy='redirectedFromInactiveAd']") 
            is not null;
}