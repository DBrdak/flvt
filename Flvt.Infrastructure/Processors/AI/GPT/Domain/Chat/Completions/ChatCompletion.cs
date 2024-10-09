namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Chat.Completions;

internal sealed record ChatCompletion(
    string Id,
    string Object,
    long Created,
    string Model,
    string SystemFingerprint,
    IReadOnlyCollection<Choice> Choices,
    Usage Usage
);