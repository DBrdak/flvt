using Flvt.Domain.ScrapedAdvertisements;

namespace Flvt.Infrastructure.Scrapers.Domiporta;

internal sealed record DomiportaAdContent(
    Dictionary<string, string> Features,
    string Description) : ScrapedAdContent;