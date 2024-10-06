namespace Flvt.Domain.Primitives;

public sealed record Currency
{
    public static readonly Currency Pln = new("PLN");
    public static readonly Currency AltPln = new("zł");

    [Newtonsoft.Json.JsonConstructor]
    [System.Text.Json.Serialization.JsonConstructor]
    private Currency(string code) => Code = code;

    public string Code { get; init; }

    public static Result<Currency> FromCode(string code)
    {
        return All.FirstOrDefault(c => c.Code.ToLower() == code.ToLower()) ??
               Result.Failure<Currency>(new($"Currency {code} not found"));
    }

    public static readonly IReadOnlyCollection<Currency> All =
    [
        Pln,
        AltPln
    ];
}