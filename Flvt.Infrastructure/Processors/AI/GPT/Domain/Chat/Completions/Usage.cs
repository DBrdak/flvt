namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Chat.Completions;

internal sealed record Usage(
    int PromptTokens,
    int CompletionTokens,
    int TotalTokens,
    CompletionTokensDetails CompletionTokensDetails
);