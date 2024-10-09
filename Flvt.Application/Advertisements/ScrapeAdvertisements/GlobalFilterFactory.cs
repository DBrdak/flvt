using Flvt.Domain.Primitives.Subscribers.Filters;

namespace Flvt.Application.Advertisements.ScrapeAdvertisements;

internal sealed class GlobalFilterFactory
{
    public static List<Filter> CreateFiltersForAllLocations()
    {
        var filters = FilterLocation.SupportedCities.Select(
            (city, count) => Filter.Create(
                count.ToString(),
                city.City,
                null,
                null,
                null,
                null,
                null,
                null,
                null));

        return filters.Select(filter => filter.Value).ToList();
    }
}