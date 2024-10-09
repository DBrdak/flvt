namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Runs.ValueObjects;

internal sealed record TruncationStrategy(
    string Method,
    int MaxTokens
);