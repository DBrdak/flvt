namespace Flvt.Processor.AI.GPT.Models.Response;

internal sealed record Message(
    string Role,
    string Content
);