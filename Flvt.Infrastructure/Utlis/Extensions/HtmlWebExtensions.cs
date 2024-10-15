using Flvt.Infrastructure.Scrapers.Shared;
using HtmlAgilityPack;
using Serilog;

namespace Flvt.Infrastructure.Utlis.Extensions;

internal static class HtmlWebExtensions
{
    public static async Task<HtmlDocument> SafelyLoadFromUrlAsync(
        this HtmlWeb web,
        string url,
        AdvertisementParser advertisementParser)
    {
        var htmlDoc = await web.LoadFromWebAsync(url);

        while (advertisementParser.IsRateLimitExceeded(htmlDoc))
        {
            Log.Logger.Warning(
                "Rate limit exceeded, waiting 30 seconds before trying again: {link}", url);

            await Task.Delay(30_000);
            htmlDoc = await web.LoadFromWebAsync(url);
        }

        return htmlDoc;
    }
}