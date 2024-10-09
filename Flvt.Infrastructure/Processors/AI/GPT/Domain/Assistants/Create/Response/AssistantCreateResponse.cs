using Flvt.Infrastructure.Processors.AI.GPT.Domain.Shared;
using Flvt.Infrastructure.Processors.AI.GPT.Utils;

namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Assistants.Create.Response;

internal sealed record AssistantCreateResponse(
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
    object? ResponseFormat)
{
    public Assistant AsAssistant() => new(
        Id,
        Object,
        CreatedAt,
        Name,
        Description,
        Model,
        Instructions,
        Tools,
        ToolResources,
        Metadata,
        Temperature,
        TopP,
        ResponseFormat is string ?
            new ResponseFormat(
                ResponseFormat.ToString() ??
                throw new NullReferenceException(
                    "Parameter response format missing in AssistantCreateResponse"),
                null) :
            GPTJsonConvert.DeserializeObject<ResponseFormat>(GPTJsonConvert.Serialize(ResponseFormat))
    );
}