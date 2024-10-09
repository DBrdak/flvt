using Flvt.Infrastructure.Processors.AI.GPT.Domain.Shared;

namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Assistants;

internal sealed record Assistant(
    string Id,
    string Object,
    long CreatedAt,
    string? Name,
    string? Description,
    string Model,
    string? Instructions,
    List<Tool> Tools,
    ToolResources? ToolResources,
    Dictionary<string, string>? Metadata,
    decimal? Temperature,
    decimal? TopP,
    ResponseFormat? ResponseFormat);