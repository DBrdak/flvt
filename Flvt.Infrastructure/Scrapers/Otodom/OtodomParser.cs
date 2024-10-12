using Flvt.Domain.Extensions;
using Flvt.Domain.Primitives.Advertisements;
using Flvt.Domain.Primitives.Subscribers.Filters;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure.Scrapers.Shared;
using Newtonsoft.Json;

namespace Flvt.Infrastructure.Scrapers.Otodom;

internal sealed class OtodomParser : AdvertisementParser
{
    private OtodomAdContent? _content;

    protected override string GetAdvertisementNodeSelector() => "//a[@data-cy='listing-item-link']";

    protected override string GetContentNodeSelector() => "//script[@id='__NEXT_DATA__']";

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

        for (var i = filter.MinRooms?.Value ?? 1; i <= 6 && i <= (filter.MaxRooms?.Value ?? 6); i++)
        {
            roomsValues.Add(i == 6 ? sixRoomsValue : i.ToStandarizedString());
        }

        var rooms = $"roomsNumber=[{string.Join(',', roomsValues)}]";

        return $"{GetBaseUrl()}/{GetBaseQueryRelativeUrl()}/{location}/?{minPrice}&{maxPrice}&{minArea}&{maxArea}&{rooms}";
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

    public override ScrapedContent ParseContent()
    {
        var nodeJson = Document.DocumentNode.SelectSingleNode(GetContentNodeSelector()).InnerHtml;

        _content = OtodomAdContent.FromJson(nodeJson);

        return _content;
    }

    public override IEnumerable<string> ParsePhotos() => _content?.Images.Select(image => image.Large) ?? [];
}

internal sealed record OtodomAdContent(
    string AdvertiserType,
    string CreatedAt,
    string ModifiedAt,
    string Description,
    string[] Features,
    string Title,
    OtodomInformation[] TopInformation,
    OtodomInformation[] AdditionalInformation,
    string Status,
    OtodomCharacteristics[] Characteristics,
    OtodomImage[] Images,
    OtodomLocation Location) : ScrapedContent
{
    public static OtodomAdContent FromJson(string json)
    {
        dynamic dirtyContent = JsonConvert.DeserializeObject(json);
        var cleanJson = JsonConvert.SerializeObject(dirtyContent.props.pageProps.ad);

        return JsonConvert.DeserializeObject<OtodomAdContent>(cleanJson);
    }
}

internal sealed record OtodomLocation(
    Coordinates Coordinates,
    OtodomAddress Address
    );

internal sealed record OtodomAddress(
    OtodomAddressUnit Street,
    OtodomAddressUnit Subdistrict,
    OtodomAddressUnit District,
    OtodomAddressUnit City,
    OtodomAddressUnit Municipality,
    OtodomAddressUnit County,
    OtodomAddressUnit Province);

internal sealed record OtodomAddressUnit(string Name);

internal sealed record OtodomImage(string Large);

internal sealed record OtodomCharacteristics(
    string Label,
    string LocalizedValue);

internal sealed record OtodomInformation(
    string Label,
    string[] Values,
    string Unit);