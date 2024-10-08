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
    ResponseFormat? ResponseFormat
);

internal sealed record ToolResources(
    List<string>? FileIds,
    List<string>? VectorStoreIds
);
internal sealed record ResponseFormat(
    string Type,
    string? JsonSchema
)
{
    public static ResponseFormat JsonObject = new("json_object", null);
}

internal sealed record Tool(
    string Type
);