using Flvt.Infrastructure.Processors.AI.GPT.Domain.Batches;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Chat.Completions;
using Serilog;

namespace Flvt.Infrastructure.Monitoring;

internal sealed class GPTMonitor : IPerformanceMonitor, ICostsMonitor
{
    private readonly List<ChatCompletion> _completions = [];
    private readonly List<Batch> _batches = [];

    public async Task LogPerformanceAsync()
    {
        var failedBatchesCount = _batches.Count(batch => batch.IsFailed);
        var successfulBatchesCount = _batches.Count(batch => batch.IsCompleted);
        var failedRequestsCount = _batches.Sum(batch => batch.RequestCounts.Failed);
        var completedRequestsCount = _batches.Sum(batch => batch.RequestCounts.Completed);
        var totalRequestsCount = _batches.Sum(batch => batch.RequestCounts.Total);
        var averageBatchTime = _batches
            .Where(batch => batch.CompletedAt is not null)
            .Sum(batch => batch.CompletedAt - batch.CreatedAt) / _batches.Count;

        var accuracy = (completedRequestsCount / totalRequestsCount).ToString("P");

        Log.Logger
            .ForContext<GPTMonitor>()
            .Information(
            """
            === GPT Performance Analysis ===
            Failed batches: {failedBatches}
            Successful batches: {successfulBatches}
            Failed requests: {failedRequests}
            Completeted requests: {completedRequests}
            Total requests: {totalRequests}
            
            Accuracy: {accuracy}
            Average batch time: {averageBatchTime}
            """,
            failedBatchesCount,
            successfulBatchesCount,
            failedRequestsCount,
            completedRequestsCount,
            totalRequestsCount,
            accuracy,
            averageBatchTime);
    }

    public async Task ReportCostsAsync()
    {
        var usage = _completions.Select(completion => completion.Usage).ToList();
        Log.Logger.Information("Usage: {usage}", usage); // TODO temp
        var inputTokens = usage.Sum(u => u.PromptTokens);
        var outputTokens = usage.Sum(u => u.CompletionTokens);
        var totalTokens = usage.Sum(u => u.TotalTokens);

        Log.Logger
            .ForContext<GPTMonitor>()
            .Information(
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