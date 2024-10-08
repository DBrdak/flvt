namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Threads.Create.Request;

internal sealed record ThreadCreateRequest(IEnumerable<MessageBody> Messages);