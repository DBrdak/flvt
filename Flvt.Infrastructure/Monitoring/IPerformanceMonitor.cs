namespace Flvt.Infrastructure.Monitoring;

internal interface IPerformanceMonitor : IAsyncDisposable
{
    protected Task ReportPerformanceAsync();
}