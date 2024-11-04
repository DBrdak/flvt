namespace Flvt.Infrastructure.Processors.AI.GPT.Utils;

internal sealed class GPTModel
{
    public string Value { get; init; }

    private GPTModel(string value) => Value = value;

    public static GPTModel Mini4oFineTuned => new("ft:gpt-4o-mini:my-org:custom_suffix:flvt-1");
    public static GPTModel Mini4o => new("gpt-4o-mini");
    public const string Mini4oPrimitive = "gpt-4o-mini";
}