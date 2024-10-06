namespace Flvt.Processor.AI.GPT.Models.Response;

internal sealed record Choice(
    int Index,
    Message Message,
    object? Logprobs,
    string FinishReason
);