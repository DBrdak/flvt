namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Runs.CreateAndRunThread.Request;

internal sealed record CreateAndRunThreadRequest(string AssistantId, GPTThreadCreateModel Thread);

internal sealed record GPTThreadCreateModel(IEnumerable<MessageBody> Messages);