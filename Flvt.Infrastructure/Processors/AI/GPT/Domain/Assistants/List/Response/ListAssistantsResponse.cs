namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Assistants.List.Response;

internal sealed record ListAssistantsResponse(string Object, IEnumerable<Assistant> Data);