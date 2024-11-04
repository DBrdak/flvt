using Flvt.Domain.Primitives.Responses;

namespace Flvt.Domain.Filters;

//TODO Implement
public sealed record Preferences
{
    public string Name { get; init; }

    private Preferences(string name)
    {
        Name = name;
    }

    public static Result<Preferences> Create(string value)
    {
        return new Preferences(value);
    }
}