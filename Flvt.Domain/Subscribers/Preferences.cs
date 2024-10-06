using System.Text.RegularExpressions;
using Flvt.Domain.Primitives;

namespace Flvt.Domain.Subscribers;

public sealed record Preferences
{
    public string Value { get; init; }
    private const string pattern = @"^[\p{L}\p{N},./;'[\]{}:""<>?~!@#$%^&*()`_+\-=\|]{1,255}$";
    private static Error InvalidValue =>
        new("Preferences value must have up to 255 characters and does not contain any unusual signs");

    private Preferences(string value)
    {
        Value = value;
    }

    public static Result<Preferences> Create(string value)
    {
        if (!Regex.IsMatch(value, pattern))
        {
            return InvalidValue;
        }

        return new Preferences(value);
    }
}