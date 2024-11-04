using Flvt.Domain.Primitives.Advertisements;
using Flvt.Domain.ScrapedAdvertisements;
using Newtonsoft.Json;

namespace Flvt.Infrastructure.Scrapers.Otodom;

internal sealed record OtodomAdAdContent(
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
    OtodomLocation Location) : ScrapedAdContent
{
    public static OtodomAdAdContent FromJson(string json)
    {
        dynamic dirtyContent = JsonConvert.DeserializeObject(json);
        var cleanJson = JsonConvert.SerializeObject(dirtyContent.props.pageProps.ad);

        return JsonConvert.DeserializeObject<OtodomAdAdContent>(cleanJson);
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