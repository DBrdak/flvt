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
    long CreatedAt,
    object InProgressAt,
    object ExpiresAt,
    object FinalizingAt,
    object CompletedAt,
    object FailedAt,
    object ExpiredAt,
    object CancellingAt,
    object CancelledAt,
    RequestCounts RequestCounts,
    Dictionary<string, string> Metadata
);

internal sealed record RequestCounts(
    int Total,
    int Completed,
    int Failed
);