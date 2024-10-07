using Flvt.Domain.Primitives;
using Serilog;

namespace Flvt.Infrastructure.Processors.AI.Models;

internal sealed record Prompt
{
    public string Value { get; init; }
    private const int valueMaxLength = 5000;
    private static readonly Error promptTooLongError = new("Prompt value too long");

    private Prompt(string value)
    {
        Value = value;
    }

    internal static Result<Prompt> Create(string value)
    {
        if (value.Length > valueMaxLength)
        {
            return Result.Failure<Prompt>(promptTooLongError);
        }

        return new Prompt(value);
    }

    internal static Prompt ForceCreate(string value)
    {
        if (value.Length > valueMaxLength)
        {
            Log.Logger.Warning($"A very long prompt was given - {value.Length} characters");
        }

        return new Prompt(value);
    }
}