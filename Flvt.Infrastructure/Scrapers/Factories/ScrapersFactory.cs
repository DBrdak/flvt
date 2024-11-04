using Flvt.Infrastructure.Scrapers.Olx;
using Flvt.Infrastructure.Scrapers.Otodom;
using Flvt.Infrastructure.Scrapers.Shared;

namespace Flvt.Infrastructure.Scrapers.Factories;

internal sealed class ScrapersFactory
{
    public static AdvertisementParser? GetAdvertisementParserFromUrl(string url) =>
        url switch
        {
            _ when url.Contains("otodom.pl") => new OtodomParser(),
            _ when url.Contains("olx.pl") => new OlxParser(),
            _ => null
        };
}