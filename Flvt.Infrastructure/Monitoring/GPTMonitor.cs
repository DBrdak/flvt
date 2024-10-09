using Flvt.Infrastructure.Processors.AI.GPT.Domain.Runs;

namespace Flvt.Infrastructure.Monitoring;

internal sealed class GPTMonitor : IPerformanceMonitor, ICostsMonitor
{
    private readonly List<Run> _completedRuns = [];

    public void AddRun(Run run) => _completedRuns.Add(run);

    public async Task ReportPerformanceAsync()
    {
    }

    public async Task ReportCostsAsync()
    {
    }

    public async ValueTask DisposeAsync()
    {
        await Task.WhenAll(ReportPerformanceAsync(), ReportCostsAsync());
    }
}