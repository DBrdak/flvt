namespace Flvt.Infrastructure.Monitoring;

internal interface ICostsMonitor
{
    Task ReportCostsAsync();
}