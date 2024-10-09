namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Chat.Completions;

internal sealed record Message(string Role, string Content);