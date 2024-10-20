using Flvt.Domain.Primitives.Filters.Erros;
using Flvt.Domain.Primitives.Responses;

namespace Flvt.Domain.Primitives.Filters;

public sealed record FilterPrice
{
    public decimal Value { get; init; }
    private const decimal minValue = 1.0m;
    private const decimal maxValue = 100_000_000.0m;

    private FilterPrice(decimal value)
    {
        Value = value;
    }

    internal static Result<FilterPrice> Create(decimal value)
    {
        if (value is < minValue or > maxValue)
        {
            return FilterErrors.InvalidPrice;
        }

        return new FilterPrice(value);
    }
    public override string ToString() => $"{Value}";
}