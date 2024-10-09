using System.Text.RegularExpressions;
using Flvt.Domain.Primitives.Responses;

namespace Flvt.Domain.Primitives.Subscribers.Filters;

//TODO Implement
public sealed record Preferences
{
    public string Value { get; init; }

    private Preferences(string value)
    {
        Value = value;
    }

    public static Result<Preferences> Create(string value)
    {
        return new Preferences(value);
    }
}