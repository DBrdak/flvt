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
    ToolChoice ToolChoice,
    bool ParallelToolCalls,
    ResponseFormat ResponseFormat
)
{
    public bool IsCompleted => Status == RunStatus.Completed;
    public bool IsFailed => Status != RunStatus.Completed && Status != RunStatus.InProgress && Status != RunStatus.Queued;
}

internal sealed record RequiredAction(
    string Type,
    string Description
);

internal sealed record LastError(
    string Code,
    string Message
);

internal sealed record IncompleteDetails(
    string Reason,
    string Details
);

internal sealed record Tool(
    string Type
);

internal sealed record Usage(
    int PromptTokens,
    int CompletionTokens,
    int TotalTokens
);

internal sealed record TruncationStrategy(
    string Method,
    int MaxTokens
);

internal sealed record ToolChoice(
    string Type,
    Function? Function
);

internal sealed record Function(
    string Name
);

internal sealed record ResponseFormat(
    string Type,
    JsonSchema? JsonSchema
);

internal sealed record JsonSchema(
    Dictionary<string, object> Schema
);
