using Flvt.Infrastructure.Processors.AI.GPT.Domain.Shared;

namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Assistants.Create.Request;

internal sealed record AssistantCreateRequest(
    string Model,
    string? Name,
    string? Description,
    string? Instructions,
    List<Tool> Tools,
    ToolResources? ToolResources,
    Dictionary<string, string>? Metadata,
    decimal? Temperature,
    decimal? TopP,
    ResponseFormat? ResponseFormat
);
