namespace Flvt.Processor.AI.GPT.Models.Request;

internal sealed record Message(
    string Role,
    string Content
);