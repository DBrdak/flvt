namespace Flvt.Infrastructure.Processors.AI.GPT.Utils;

internal sealed record AssistantVariant
{
    public string Name { get; init; }

    private AssistantVariant(string name)
    {
        Name = name;
    }

    public static AssistantVariant BasicAdvertisementProcessor = new("basic_advertisement_processor");
}