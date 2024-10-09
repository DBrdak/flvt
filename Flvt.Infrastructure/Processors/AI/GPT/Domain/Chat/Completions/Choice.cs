namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Chat.Completions;

internal sealed record Choice(
    int Index,
    ChoiceMessage Message,
    object? Logprobs,
    string FinishReason
);