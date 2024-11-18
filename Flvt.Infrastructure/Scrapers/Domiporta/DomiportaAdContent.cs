using Flvt.Domain.ScrapedAdvertisements;

namespace Flvt.Infrastructure.Scrapers.Domiporta;

internal sealed record DomiportaAdContent(
    long Id,
    Dictionary<string, string> Features,
    string Description) : ScrapedAdContent;