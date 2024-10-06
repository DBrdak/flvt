namespace Flvt.Infrastructure.Processors.AI.GPT.Models.Response;

internal sealed record Usage(
    int PromptTokens,
    int CompletionTokens,
    int TotalTokens,
    CompletionTokensDetails CompletionTokensDetails
);