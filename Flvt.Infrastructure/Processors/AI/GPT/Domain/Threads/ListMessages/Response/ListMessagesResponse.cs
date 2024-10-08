using Flvt.Infrastructure.Processors.AI.GPT.Domain.Messages;

namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Threads.ListMessages.Response;

internal sealed record ListMessagesResponse(string Object, IEnumerable<Message> Data);