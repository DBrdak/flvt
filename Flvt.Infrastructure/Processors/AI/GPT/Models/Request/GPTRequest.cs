namespace Flvt.Infrastructure.Processors.AI.GPT.Models.Request;

internal sealed record GPTRequest(
    string Model,
    List<Message> Messages
);