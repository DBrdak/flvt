namespace Flvt.Infrastructure.Monitoring;

internal interface IPerformanceMonitor
{
    Task ReportPerformanceAsync();
}