using System.Diagnostics;
using Flvt.Infrastructure.Scrapers.Otodom;
using Flvt.Infrastructure.Scrapers.Shared;
using Serilog;
using Serilog.Extensions.Logging;

namespace Flvt.Infrastructure.Monitoring;

internal class ScrapingMonitor : IPerformanceMonitor
{
    private readonly Stopwatch _stopwatch = new();
    private readonly AdvertisementScraper? _scraper;
    private readonly ILogger _logger;

    public ScrapingMonitor(AdvertisementScraper? scraper)
    {
        _logger = Log.Logger.ForContext(GetType());
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

        var accuracy = string.Empty;
        var percentageAccuracy = 1.0m;

        if (_scraper?.SuccessfullyScrapedAds is var successfullyScrapedAds &&
            _scraper?.SuccessfullyScrapedLinks is var successfullyScrapedLinks &&
            successfullyScrapedAds is not null &&
            successfullyScrapedLinks is not null && successfullyScrapedLinks != 0)
        {
            percentageAccuracy = (decimal)(successfullyScrapedAds / successfullyScrapedAds);
        }

        accuracy = percentageAccuracy.ToString("P");

        _logger.Information(
            """
            === Scraper performance analysis ===
            Website: {website}
            Scraped advertisements: {adsCount} 
            Scraped links: {linksCount}
            Accuracy: {accuracy}
            Time: {time} ms
            Filters: {filter}
            """,
            _scraper?.GetType().Name.Replace("Scraper", ""),
            _scraper?.SuccessfullyScrapedAds,
            _scraper?.SuccessfullyScrapedLinks,
            accuracy,
            elapsed.Milliseconds,
            _scraper?.Filter);

    }
}