using Flvt.Infrastructure.Processors.AI.GPT.Domain.Assistants.Create.Response;

namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Assistants.List.Response;

internal sealed record ListAssistantsResponse(string Object, IEnumerable<AssistantCreateResponse> Data);