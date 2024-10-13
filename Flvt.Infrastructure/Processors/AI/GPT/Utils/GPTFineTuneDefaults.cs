namespace Flvt.Infrastructure.Processors.AI.GPT.Utils;

internal static class GPTFineTuneDefaults
{
    public const decimal LowTemperature = 0.15m;
    public const decimal MidTemperature = 0.4m;
    public const decimal HighTemperature = 0.9m;

    public const decimal LowTopP = 0.5m;
    public const decimal MidTopP = 0.7m;
    public const decimal HighTopP = 0.9m;
}