using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.Primitives.Subscribers.Filters.Erros;

namespace Flvt.Domain.Primitives.Subscribers.Filters;

public sealed record FilterRoomsCount
{
    public int Value { get; init; }
    private const int minValue = 1;
    private const int maxValue = 6;

    private FilterRoomsCount(int value)
    {
        Value = value;
    }

    internal static Result<FilterRoomsCount> Create(int value)
    {
        if (value is < minValue or > maxValue)
        {
            return FilterErrors.InvalidRoomsCount;
        }

        return new FilterRoomsCount(value);
    }
}