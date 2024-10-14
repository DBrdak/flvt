using Flvt.Domain.Primitives.Responses;

namespace Flvt.Domain.Primitives.Advertisements;

public sealed record Country
{
    public string Name { get; init; }
    public string NativeName { get; init; }
    public string Code { get; init; }

    private Country(string name, string nativeName, string code)
    {
        Name = name;
        Code = code;
        NativeName = nativeName;
    }

    public static readonly Country Poland = new("Poland", "Polska", "PL");

    public static readonly IReadOnlyCollection<Country> All =
    [
        Poland
    ];

    private static Error InvalidCodeError(string code) => new ($"Country with code {code} does not exist.");

    public static Result<Country> Create(string code)
    {
        var country = All.FirstOrDefault(x => x.Code == code);

        if (country is null)
        {
            return InvalidCodeError(code);
        }

        return country;
    }
}