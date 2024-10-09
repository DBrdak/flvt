namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Shared;

internal sealed record ResponseFormat(
    string Type,
    string? JsonSchema)
{

    public static ResponseFormat JsonSchemaFormat(string jsonSchema) => new("json_schema", jsonSchema);

}