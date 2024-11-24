using System.Diagnostics;
using Flvt.Infrastructure.Scrapers.Shared.Scrapers;
using Serilog;

namespace Flvt.Infrastructure.Monitoring;

internal class LinksScrapingMonitor : IPerformanceMonitor
{
    private readonly Stopwatch _stopwatch = new();
    private readonly AdvertisementLinkScraper? _scraper;
    private readonly ILogger _logger;

    public LinksScrapingMonitor(AdvertisementLinkScraper? scraper)
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

        _logger.Information(
            """
            === Scraper performance analysis ===
            Website: {website}
            Scraped links: {adsCount}
            Time: {time} ms
            """,
            _scraper?.GetType().Name.Replace("LinksScraper", ""),
            _scraper?.SuccessfullyScrapedLinks,
            elapsed.Milliseconds);

    }
}