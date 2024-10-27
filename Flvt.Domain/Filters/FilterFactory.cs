using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.Subscribers;

namespace Flvt.Domain.Filters;

/// <summary>
/// Creates filters of a given tier.
/// </summary>
internal sealed class FilterFactory
{
    public static Result<Filter> CreateBasicFilter(
        string name,
        string city,
        decimal? minPrice,
        decimal? maxPrice,
        int? minRooms,
        int? maxRooms,
        decimal? minArea,
        decimal? maxArea)
    {
        return Filter.Create(
            name,
            city,
            minPrice,
            maxPrice,
            minRooms,
            maxRooms,
            minArea,
            maxArea,
            Frequency.Daily,
            SubscribtionTier.Basic);
    }
}