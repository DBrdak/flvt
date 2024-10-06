namespace Flvt.Domain.Primitives;

public sealed record Country
{
    public string Name { get; init; }
    public string Code { get; init; }

    private Country(string name, string code)
    {
        Name = name;
        Code = code;
    }

    public static readonly Country Poland = new("Poland", "PL");

    public static readonly IReadOnlyCollection<Country> All =
    [
        Poland
    ];
}