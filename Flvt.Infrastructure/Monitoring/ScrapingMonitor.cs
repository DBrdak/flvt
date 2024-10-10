using System.Diagnostics;
using Flvt.Domain.ScrapedAdvertisements;
using Serilog;

namespace Flvt.Infrastructure.Monitoring;

internal class ScrapingMonitor : IPerformanceMonitor
{
    private readonly Stopwatch _stopwatch = new();
    private readonly List<ScrapedAdvertisement> _morizonAds = new();
    private readonly List<ScrapedAdvertisement> _otodomAds = new();
    private readonly List<ScrapedAdvertisement> _olxAds = new();

    public ScrapingMonitor()
    {
        _stopwatch.Start();
    }

    public async ValueTask DisposeAsync()
    {
        _stopwatch.Stop();

        await ReportPerformanceAsync();
    }

    public async Task ReportPerformanceAsync()
    {
        var elapsed = _stopwatch.Elapsed;

        Log.Information(
            "Scraped {MorizonCount} Morizon ads, {OtodomCount} Otodom ads, {OlxCount} Olx ads, Total: {Total}. Operation time: {time} minutes",
            _morizonAds.Count,
            _otodomAds.Count,
            _olxAds.Count,
            _morizonAds.Count + _otodomAds.Count + _olxAds.Count,
            $"{elapsed.Minutes}:{elapsed.Seconds % 60}");
    }

    public ScrapingMonitor AddMorizon(List<ScrapedAdvertisement> morizonAds)
    {
        _morizonAds.AddRange(morizonAds);

        return this;
    }

    public ScrapingMonitor AddOtodom(List<ScrapedAdvertisement> otodomAds)
    {
        _otodomAds.AddRange(otodomAds);

        return this;
    }

    public ScrapingMonitor AddOlx(List<ScrapedAdvertisement> olxAds)
    {
        _olxAds.AddRange(olxAds);

        return this;
    }
}