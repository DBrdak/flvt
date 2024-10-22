using Flvt.Domain.Primitives.Responses;

namespace Flvt.Domain.Filters;

public sealed record Frequency
{
    public long EverySeconds { get; init; }
    public long LastUsed { get; init; }
    public long NextUse => LastUsed + EverySeconds;

    private const long oneHourInSeconds = 60 * 60;
    private const long oneDayInSeconds = oneHourInSeconds * 24;

    internal static readonly Frequency Every12H = new(oneDayInSeconds / 2, 0);
    internal static readonly Frequency Daily = new(oneDayInSeconds, 0);
    internal static readonly Frequency Every3Days = new(oneDayInSeconds * 3, 0);
    internal static readonly Frequency Weekly = new(oneDayInSeconds * 7, 0);

    private Frequency(long everySeconds, long lastUsed)
    {
        EverySeconds = everySeconds;
        LastUsed = lastUsed;
    }

    public static Result<Frequency> Create(string frequency) => frequency.ToLower() switch
    {
        "every12h" => Every12H,
        "daily" => Daily,
        "every3days" => Every3Days,
        "weekly" => Weekly,
        _ => new Error($"{frequency} is invalid frequency")
    };
}