namespace Flvt.Infrastructure.Processors.AI.GPT.Models.Request;

internal sealed record Message(
    string Role,
    string Content
);