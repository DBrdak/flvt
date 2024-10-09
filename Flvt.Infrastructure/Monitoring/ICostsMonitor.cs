namespace Flvt.Infrastructure.Monitoring;

internal interface ICostsMonitor : IAsyncDisposable
{
    Task ReportCostsAsync();
}