namespace Flvt.Processor.AI.GPT.Models.Request;

internal sealed record GPTRequest(
    string Model,
    List<Message> Messages
);