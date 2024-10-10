using System.Globalization;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.Primitives.Subscribers.Filters.Erros;

namespace Flvt.Domain.Primitives.Subscribers.Filters;

public sealed record FilterArea
{
    public decimal Value { get; init; }
    private const decimal minValue = 1.0m;
    private const decimal maxValue = 1000.0m;

    private FilterArea(decimal value)
    {
        Value = value;
    }

    internal static Result<FilterArea> Create(decimal value)
    {
        if (value is < minValue or > maxValue)
        {
            return FilterErrors.InvalidArea;
        }

        return new FilterArea(value);
    }

    public override string ToString() => $"{Value}";
}