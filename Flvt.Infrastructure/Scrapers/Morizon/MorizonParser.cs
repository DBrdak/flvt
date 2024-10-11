using Flvt.Domain.Extensions;
using Flvt.Domain.Primitives.Subscribers.Filters;
using Flvt.Infrastructure.Scrapers.Shared;

namespace Flvt.Infrastructure.Scrapers.Morizon;

internal class MorizonParser : AdvertisementParser
{
    public MorizonParser() : base()
    {
    }

    protected override string GetAdvertisementNodeSelector() =>
        "//div[@class='card__outer']/a[@data-cy='propertyUrl']";

    protected override string GetContentNodeSelector() => "//div[@id='slot-panel']";

    protected override string GetBaseUrl() => "https://www.morizon.pl";

    protected override string GetBaseQueryRelativeUrl() => "do-wynajecia/mieszkania";

    public override string ParseQueryUrl(Filter filter)
    {
        var location = filter.Location.City.ToLower().ReplacePolishCharacters();

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
        var advertisements = Document.DocumentNode.SelectNodes(GetAdvertisementNodeSelector()).ToList();

        return advertisements.Select(
                ad => string.Concat(
                    GetBaseUrl(),
                    ad.GetAttributeValue(
                        "href",
                        string.Empty)))
            .ToList();
    }

    public override string ParseContent()
    {
        var contentNode = Document.DocumentNode.SelectSingleNode(GetContentNodeSelector());
        //var photosHtml = contentNode.PreviousSibling.PreviousSibling.OuterHtml;
        var contentHtml = contentNode?.ParentNode?.InnerHtml;

        //return string.Join('\n', photosHtml, contentHtml);
        return contentHtml;
    }
}