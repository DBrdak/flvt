using System.Diagnostics;
using Flvt.Infrastructure.Scrapers.Otodom;
using Serilog;

namespace Flvt.Infrastructure.Monitoring;

internal class ScrapingMonitor : IPerformanceMonitor
{
    private readonly Stopwatch _stopwatch = new();
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

        var otodomAdsCount = _otodomScraper?.SuccessfullyScrapedAds;
        var otodomLinksCount = _otodomScraper?.SuccessfullyScrapedLinks;

        Log.Information(
            """
            Scraped [ads / links]:
                - [OTODOM] {OtodomCount} / {OtodomLinks}
            
            Total: {Ads} / {Links}. 
            Operation time: {time} minutes
            """,
            otodomAdsCount,
            otodomLinksCount,
            otodomAdsCount,
            otodomLinksCount,
            $"{elapsed.Minutes}:{elapsed.Seconds % 60}");

    }

    public ScrapingMonitor AddOtodom(OtodomScraper otodomScraper)
    {
        _otodomScraper = otodomScraper;

        return this;
    }
}