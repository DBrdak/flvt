using System.Diagnostics;
using Flvt.Infrastructure.Scrapers.Otodom;
using Flvt.Infrastructure.Scrapers.Shared;
using Serilog;

namespace Flvt.Infrastructure.Monitoring;

internal class ScrapingMonitor : IPerformanceMonitor
{
    private readonly Stopwatch _stopwatch = new();
    private readonly AdvertisementScraper? _scraper;

    public ScrapingMonitor(AdvertisementScraper? scraper)
    {
        _scraper = scraper;
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

        var otodomAdsCount = _scraper?.SuccessfullyScrapedAds;
        var otodomLinksCount = _scraper?.SuccessfullyScrapedLinks;

        Log.Information(
            """
            Scraped [ads / links]: {OtodomCount} / {OtodomLinks} from {scraper}
            
            Total: {Ads} / {Links}. 
            Operation time: {time} minutes
            """,
            otodomAdsCount,
            otodomLinksCount,
            _scraper?.GetType().Name,
            otodomAdsCount,
            otodomLinksCount,
            $"{elapsed.Minutes}:{elapsed.Seconds % 60}");

    }
}