using Flvt.Infrastructure.Processors.AI.GPT.Domain.Chat.Completions;

namespace Flvt.Infrastructure.Monitoring;

internal sealed class GPTMonitor : IPerformanceMonitor, ICostsMonitor
{
    private readonly List<ChatCompletion> _completions = [];

    public void AddCompletion(ChatCompletion run) => _completions.Add(run);

    public async Task LogPerformanceAsync()
    {
    }

    public async Task ReportCostsAsync()
    {
    }

    public async ValueTask DisposeAsync()
    {
        await Task.WhenAll(LogPerformanceAsync(), ReportCostsAsync());
    }
}