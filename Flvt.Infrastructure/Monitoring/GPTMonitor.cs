using Flvt.Infrastructure.Processors.AI.GPT.Domain.Batches;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Chat.Completions;
using Serilog;

namespace Flvt.Infrastructure.Monitoring;

internal sealed class GPTMonitor : IPerformanceMonitor, ICostsMonitor
{
    private readonly List<ChatCompletion> _completions = [];
    private readonly List<Batch> _batches = [];
    private readonly ILogger _logger = Log.Logger.ForContext<GPTMonitor>();

    public async Task LogPerformanceAsync()
    {
        var failedRequestsCount = _batches.Select(batch => batch.RequestCounts.Failed).Count();
        var completedRequestsCount = _batches.Select(batch => batch.RequestCounts.Completed).Count();
        var totalRequestsCount = _batches.Select(batch => batch.RequestCounts.Total).Count();
        var averageBatchTime = _batches
            .Where(batch => batch.CompletedAt is not null)
            .Sum(batch => batch.CompletedAt - batch.CreatedAt) / _batches.Count;

        var accuracy = (completedRequestsCount / totalRequestsCount).ToString("P");

        _logger.Information(
            """
            === GPT Performance Analysis ===
            Failed requests: {failedRequests}
            Completeted requests: {completedRequests}
            Total requests: {totalRequests}
            
            Accuracy: {accuracy}
            Average batch time: {averageBatchTime}
            """,
            failedRequestsCount,
            completedRequestsCount,
            totalRequestsCount,
            accuracy,
            averageBatchTime);
    }

    public async Task ReportCostsAsync()
    {
        var usage = _completions.Select(completion => completion.Usage).ToList();
        var inputTokens = usage.Sum(u => u.PromptTokens);
        var outputTokens = usage.Sum(u => u.CompletionTokens);
        var totalTokens = usage.Sum(u => u.TotalTokens);

        _logger.Information(
            """
            === GPT Cost Analysis ===
            Input tokens: {inputTokens}
            Output tokens: {outputTokens}
            Total tokens: {totalCost}
            """,
            inputTokens,
            outputTokens,
            totalTokens);
    }

    public void AddCompletion(ChatCompletion completion) => _completions.Add(completion);
    public void AddBatch(Batch batch) => _batches.Add(batch);

    public async ValueTask DisposeAsync()
    {
        await Task.WhenAll(LogPerformanceAsync(), ReportCostsAsync());
    }
}