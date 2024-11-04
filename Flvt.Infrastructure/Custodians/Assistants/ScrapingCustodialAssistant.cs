using Flvt.Infrastructure.Scrapers.Factories;
using Flvt.Infrastructure.Utlis.Extensions;
using HtmlAgilityPack;
using Serilog;

namespace Flvt.Infrastructure.Custodians.Assistants;

internal sealed class ScrapingCustodialAssistant
{
    private readonly HtmlWeb _web = new();

    public async Task<string?> ChekIfAdvertisementIsOutdatedAsync(string advertisementUrl)
    {
        var parser = ScrapersFactory.GetAdvertisementParserFromUrl(advertisementUrl);

        if (parser is null)
        {
            Log.Logger.Error(
                "Not identified advertisements page: {url}, parser is not available",
                advertisementUrl);

            return null;
        }

        var document = await _web.SafelyLoadFromUrlAsync(advertisementUrl, parser);
        parser.SetHtmlDocument(document);

        return parser.IsOutdatedAdvertisement() ? advertisementUrl : null;
    }
}