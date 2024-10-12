using System.Diagnostics;
using Flvt.Infrastructure.Scrapers.Olx;
using Flvt.Infrastructure.Scrapers.Otodom;
using Serilog;

namespace Flvt.Infrastructure.Monitoring;

internal class ScrapingMonitor : IPerformanceMonitor
{
    private readonly Stopwatch _stopwatch = new();
    private OlxScraper? _olxScraper;
    private OtodomScraper? _otodomScraper;

    public ScrapingMonitor()
    {
        _stopwatch.Start();
    }

    public async ValueTask DisposeAsync()
    {
        _stopwatch.Stop();

        await LogPerformanceAsync();
    }

    public async Task LogPerformanceAsync()
    {
        var elapsed = _stopwatch.Elapsed;

        var olxAdsCount = _olxScraper?.SuccessfullyScrapedAds;
        var olxLinksCount = _olxScraper?.SuccessfullyScrapedLinks;
        var olxTotalLinks = _olxScraper?.SuccessfullyScrapedLinks + _olxScraper?.UnsuccessfullyScrapedLinks;

        var otodomAdsCount = _otodomScraper?.SuccessfullyScrapedAds;
        var otodomLinksCount = _otodomScraper?.SuccessfullyScrapedLinks;
        var otodomTotalLinks = _otodomScraper?.SuccessfullyScrapedLinks + _otodomScraper?.UnsuccessfullyScrapedLinks;

        Log.Information(
            """
            Scraped [ads / links / totalLinks]:
                - [OTODOM] {OtodomCount} / {OtodomLinks} / {OtodomTotalLinks}
                - [OLX] {OlxCount} / {OlxLinks} / {OlxTotalLinks}
            
            Total: {Ads} / {Links} / {TotalLinks}. 
            Operation time: {time} minutes
            """,
            olxAdsCount,
            olxLinksCount,
            olxTotalLinks,
            otodomAdsCount,
            otodomLinksCount,
            otodomTotalLinks,
            olxAdsCount + otodomAdsCount,
            olxLinksCount + otodomLinksCount,
            olxTotalLinks + otodomTotalLinks,
            $"{elapsed.Minutes}:{elapsed.Seconds % 60}");

    }

    public ScrapingMonitor AddOlx(OlxScraper olxScraper)
    {
        _olxScraper = olxScraper;

        return this;
    }

    public ScrapingMonitor AddOtodom(OtodomScraper otodomScraper)
    {
        _otodomScraper = otodomScraper;

        return this;
    }
}