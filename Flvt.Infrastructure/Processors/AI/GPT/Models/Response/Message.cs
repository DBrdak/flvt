namespace Flvt.Infrastructure.Processors.AI.GPT.Models.Response;

internal sealed record Message(
    string Role,
    string Content
);