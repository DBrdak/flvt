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
        var htmlDoc = await web.TryLoadFromWebAsync(url);

        while (htmlDoc is null || advertisementParser.IsRateLimitExceeded(htmlDoc))
        {
            Log.Logger.Warning(
                "Rate limit exceeded, waiting 30 seconds before trying again: {link}", url);

            await Task.Delay(30_000);
            htmlDoc = await web.LoadFromWebAsync(url);
        }

        return htmlDoc;
    }

    public static async Task<HtmlDocument?> TryLoadFromWebAsync(this HtmlWeb web, string url)
    {
        try
        {
            return await web.LoadFromWebAsync(url);
        }
        catch (HttpRequestException e)
        {
            Log.Logger.Error(
                "Error occured when trying to retrieve response from: {link} - Code: {errorCode}, Type: {errorType}, Message: {errorMessage}",
                url,
                e.StatusCode,
                e.HttpRequestError,
                e.Message);
            
            return null;
        }
    }
}