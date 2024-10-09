using Flvt.Infrastructure.Processors.AI.GPT.Domain.Runs.ValueObjects;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Shared;

namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Runs;

internal sealed record Run(
    string Id,
    string Object,
    long CreatedAt,
    string ThreadId,
    string AssistantId,
    string Status,
    RequiredAction? RequiredAction,
    LastError? LastError,
    long? ExpiresAt,
    long? StartedAt,
    long? CancelledAt,
    long? FailedAt,
    long? CompletedAt,
    IncompleteDetails? IncompleteDetails,
    string Model,
    string Instructions,
    List<Tool> Tools,
    Dictionary<string, string>? Metadata,
    Usage? Usage,
    double? Temperature,
    double? TopP,
    int? MaxPromptTokens,
    int? MaxCompletionTokens,
    TruncationStrategy TruncationStrategy,
    object ToolChoice,
    bool ParallelToolCalls,
    object ResponseFormat
)
{
    public bool IsCompleted => Status == RunStatus.Completed;
    public bool IsFailed => Status != RunStatus.Completed && Status != RunStatus.InProgress && Status != RunStatus.Queued;
    public bool IsInProgress => !IsCompleted && !IsFailed;
}