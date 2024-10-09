using Flvt.Infrastructure.Processors.AI.GPT.Domain.Assistants;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Assistants.Create.Request;
using Flvt.Infrastructure.Processors.AI.Training.Instructions;

namespace Flvt.Infrastructure.Processors.AI.GPT.Utils;

internal sealed class AssistantFactory
{
    private static AssistantCreateRequest CreateBasicAdvertisementProcessor()
    {
        return new AssistantCreateRequest(
            Model: "gpt-4o-mini",
            Name: AssistantVariant.BasicAdvertisementProcessor.Name,
            Instructions: BasicProcessorInstructions.CompleteInstruction,
            ResponseFormat: BasicProcessorInstructions.ResponseFormat,
            Tools: [],
            ToolResources: null,
            Metadata: null,
            Temperature: null,
            TopP: null,
            Description: null
        );
    }

    public static AssistantCreateRequest CreateFromVariant(AssistantVariant variant) => variant switch
    {
        _ when variant == AssistantVariant.BasicAdvertisementProcessor => CreateBasicAdvertisementProcessor(),
        _ => throw new ArgumentOutOfRangeException(nameof(variant), $"Unknown assistant variant: {variant.Name}", null)
    };
}