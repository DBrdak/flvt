using Flvt.Domain.ScrapedAdvertisements;
using Newtonsoft.Json;

namespace Flvt.Infrastructure.Scrapers.Olx;

internal sealed record OlxAdContent(
    OlxInfo Info,
    string Attributes,
    string Location) : ScrapedAdContent;

internal sealed record OlxInfo(
    string Name,
    string[] Image,
    string Description,
    OlxOffers Offers)
{
    public static OlxInfo FromJson(string json) =>
        JsonConvert.DeserializeObject<OlxInfo>(json);
}

internal sealed record OlxOffers(
    decimal Price,
    string PriceCurrency);