namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Runs.ValueObjects;

internal sealed record Usage(
    int PromptTokens,
    int CompletionTokens,
    int TotalTokens
);