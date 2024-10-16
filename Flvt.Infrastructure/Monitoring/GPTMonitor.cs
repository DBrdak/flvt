using Flvt.Infrastructure.Processors.AI.GPT.Domain.Batches;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Chat.Completions;
using Serilog;

namespace Flvt.Infrastructure.Monitoring;

internal sealed class GPTMonitor : IPerformanceMonitor, ICostsMonitor
{
    private readonly List<ChatCompletion> _completions = [];
    private readonly List<Batch> _batches = [];

    public void AddCompletion(ChatCompletion run) => _completions.Add(run);

    public async Task LogPerformanceAsync()
    {
        //todo implement
    }

    public async Task ReportCostsAsync()
    {
        //todo implement
    }

    public void AddBatch(Batch batch)
    {
        _batches.Add(batch);
    }

    public async ValueTask DisposeAsync()
    {
        await Task.WhenAll(LogPerformanceAsync(), ReportCostsAsync());
    }
}