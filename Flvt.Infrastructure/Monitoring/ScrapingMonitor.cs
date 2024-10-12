using System.Diagnostics;
using Flvt.Infrastructure.Scrapers.Morizon;
using Flvt.Infrastructure.Scrapers.Olx;
using Flvt.Infrastructure.Scrapers.Otodom;
using Serilog;

namespace Flvt.Infrastructure.Monitoring;

internal class ScrapingMonitor : IPerformanceMonitor
{
    private readonly Stopwatch _stopwatch = new();
    private MorizonScraper? _morizonScraper;
    private OtodomScraper? _otodomScraper;
    private OlxScraper? _olxScraper;

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

        var morizonAdsCount = _morizonScraper?.SuccessfullyScrapedAds;
        var morizonLinksCount = _morizonScraper?.SuccessfullyScrapedLinks;
        var morizonTotalLinks = _morizonScraper?.SuccessfullyScrapedLinks + _morizonScraper?.UnsuccessfullyScrapedLinks;

        var otodomAdsCount = _otodomScraper?.SuccessfullyScrapedAds;
        var otodomLinksCount = _otodomScraper?.SuccessfullyScrapedLinks;
        var otodomTotalLinks = _otodomScraper?.SuccessfullyScrapedLinks + _otodomScraper?.UnsuccessfullyScrapedLinks;

        var olxAdsCount = _olxScraper?.SuccessfullyScrapedAds;
        var olxLinksCount = _olxScraper?.SuccessfullyScrapedLinks;
        var olxTotalLinks = _olxScraper?.SuccessfullyScrapedLinks + _olxScraper?.UnsuccessfullyScrapedLinks;

        Log.Information(
            """
            Scraped [ads / links / totalLinks]:
                - [MORIZON] {MorizonCount} / {MorizonLinks} / {MorizonTotalLinks}
                - [OTODOM] {OtodomCount} / {OtodomLinks} / {OtodomTotalLinks}
                - [OLX] {OlxCount} / {OlxLinks} / {OlxTotalLinks}
            
            Total: {Ads} / {Links} / {TotalLinks}. 
            Operation time: {time} minutes
            """,
            morizonAdsCount,
            morizonLinksCount,
            morizonTotalLinks,
            otodomAdsCount,
            otodomLinksCount,
            otodomTotalLinks,
            olxAdsCount,
            olxLinksCount,
            olxTotalLinks,
            morizonAdsCount + otodomAdsCount + olxAdsCount,
            morizonLinksCount + otodomLinksCount + olxLinksCount,
            morizonTotalLinks + otodomTotalLinks + olxTotalLinks,
            $"{elapsed.Minutes}:{elapsed.Seconds % 60}");

    }

    public ScrapingMonitor AddMorizon(MorizonScraper morizonScraper)
    {
        _morizonScraper = morizonScraper;

        return this;
    }

    public ScrapingMonitor AddOtodom(OtodomScraper otodomScraper)
    {
        _otodomScraper = otodomScraper;

        return this;
    }

    public ScrapingMonitor AddOlx(OlxScraper olxScraper)
    {
        _olxScraper = olxScraper;

        return this;
    }
}