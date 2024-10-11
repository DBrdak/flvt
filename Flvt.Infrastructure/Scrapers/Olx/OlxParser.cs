using Flvt.Domain.Extensions;
using Flvt.Domain.Primitives.Advertisements;
using Flvt.Domain.Primitives.Subscribers.Filters;
using Flvt.Infrastructure.Scrapers.Otodom;
using Flvt.Infrastructure.Scrapers.Shared;
using HtmlAgilityPack;

namespace Flvt.Infrastructure.Scrapers.Olx;

internal sealed class OlxParser : AdvertisementParser
{
    protected override string GetAdvertisementNodeSelector() =>
        "//div[@data-cy='l-card']//a[@href and @target='_blank']";

    protected override string GetContentNodeSelector() => "//*[@data-testid='main' or @data-testid='aside']";

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
        var advertisements = Document.DocumentNode.SelectNodes(GetAdvertisementNodeSelector()).ToList();
        var relativeLinks = advertisements.Select(
            ad => ad.GetAttributeValue(
                "href",
                string.Empty))
            .Where(link => !link.ToLower().Contains("otodom"))
            .ToList();

        return relativeLinks.Distinct().Select(link => string.Concat(GetBaseUrl(), link)).ToList();
    }

    public override string ParseContent() =>
        string.Join(
            '\n',
            Document.DocumentNode.SelectNodes(GetContentNodeSelector())
                .Select(node => node.InnerHtml));
}