using Flvt.Domain.Primitives.Responses;

namespace Flvt.Domain.Filters;

public sealed record Frequency
{
    public string Name { get; init; }
    public long EverySeconds { get; init; }
    public long LastUsed { get; init; }
    public long NextUse => LastUsed + EverySeconds;

    private const long oneHourInSeconds = 60 * 60;
    private const long oneDayInSeconds = oneHourInSeconds * 24;

    internal static readonly Frequency Every12H = new(nameof(Every12H), oneDayInSeconds / 2, 0);
    internal static readonly Frequency Daily = new(nameof(Daily), oneDayInSeconds, 0);
    internal static readonly Frequency Every3Days = new(nameof(Every3Days), oneDayInSeconds * 3, 0);
    internal static readonly Frequency Weekly = new(nameof(Weekly), oneDayInSeconds * 7, 0);

    private Frequency(string name, long everySeconds, long lastUsed)
    {
        EverySeconds = everySeconds;
        LastUsed = lastUsed;
        Name = name;
    }

    public static Result<Frequency> Create(string frequency) => frequency switch
    {
        nameof(Every12H) => Every12H,
        nameof(Daily) => Daily,
        nameof(Every3Days) => Every3Days,
        nameof(Weekly) => Weekly,
        _ => new Error($"{frequency} is invalid frequency")
    };
}