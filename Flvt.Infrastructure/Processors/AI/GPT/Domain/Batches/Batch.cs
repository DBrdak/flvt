namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Batches;

internal sealed record Batch(
    string Id,
    string Object,
    string Endpoint,
    object Errors,
    string InputFileId,
    string CompletionWindow,
    string Status,
    string OutputFileId,
    string ErrorFileId,
    long? CreatedAt,
    long? InProgressAt,
    long? ExpiresAt,
    long? FinalizingAt,
    long? CompletedAt,
    long? FailedAt,
    long? ExpiredAt,
    long? CancellingAt,
    long? CancelledAt,
    RequestCounts RequestCounts,
    Dictionary<string, string> Metadata
)
{
    public bool IsCompleted => CompletedAt is not null;

    public bool IsFailed =>
        CancelledAt is not null || FailedAt is not null || CancellingAt is not null || ExpiredAt is not null;
}

internal sealed record RequestCounts(
    int Total,
    int Completed,
    int Failed
);