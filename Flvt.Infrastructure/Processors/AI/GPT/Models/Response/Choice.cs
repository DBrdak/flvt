namespace Flvt.Infrastructure.Processors.AI.GPT.Models.Response;

internal sealed record Choice(
    int Index,
    Message Message,
    object? Logprobs,
    string FinishReason
);