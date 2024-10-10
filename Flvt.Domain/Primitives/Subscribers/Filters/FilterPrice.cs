using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.Primitives.Subscribers.Filters.Erros;

namespace Flvt.Domain.Primitives.Subscribers.Filters;

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