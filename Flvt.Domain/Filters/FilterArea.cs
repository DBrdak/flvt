﻿using Flvt.Domain.Filters.Erros;
using Flvt.Domain.Primitives.Responses;

namespace Flvt.Domain.Filters;

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