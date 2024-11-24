using System.Diagnostics;
using Flvt.Infrastructure.Scrapers.Shared.Scrapers;
using Serilog;

namespace Flvt.Infrastructure.Monitoring;

internal class AdsScrapingMonitor : IPerformanceMonitor
{
    private readonly Stopwatch _stopwatch = new();
    private readonly AdvertisementScraper? _adScraper;
    private readonly ILogger _logger;

    public AdsScrapingMonitor(AdvertisementScraper? adScraper)
    {
        _logger = Log.Logger.ForContext(GetType());
        _adScraper = adScraper;
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

        _logger.Information(
            """
            === Scraper performance analysis ===
            Website: {website}
            Scraped advertisements: {adsCount}
            Time: {time} ms
            """,
            _adScraper?.GetType().Name.Replace("AdvertisementScraper", ""),
            _adScraper?.SuccessfullyScrapedAds,
            elapsed.Milliseconds);

    }
}