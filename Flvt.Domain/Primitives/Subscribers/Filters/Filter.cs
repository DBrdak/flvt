using Flvt.Domain.Primitives.Advertisements;
using Flvt.Domain.Primitives.Responses;

namespace Flvt.Domain.Primitives.Subscribers.Filters;

public sealed record Filter
{
    public FilterName Name { get; private set; }
    public FilterLocation Location { get; private set; }
    public FilterPrice? MinPrice { get; private set; }
    public FilterPrice? MaxPrice { get; private set; }
    public FilterRoomsCount? MinRooms { get; private set; }
    public FilterRoomsCount? MaxRooms { get; private set; }
    public FilterArea? MinArea { get; private set; }
    public FilterArea? MaxArea { get; private set; }
    public Preferences? Preferences { get; init; }

    public Filter(FilterName name, FilterLocation location, FilterPrice? minPrice, FilterPrice? maxPrice, FilterRoomsCount? minRooms, FilterRoomsCount? maxRooms, FilterArea? minArea, FilterArea? maxArea, Preferences? preferences)
    {
        Name = name;
        Location = location;
        MinPrice = minPrice;
        MaxPrice = maxPrice;
        MinRooms = minRooms;
        MaxRooms = maxRooms;
        MinArea = minArea;
        MaxArea = maxArea;
        Preferences = preferences;
    }

    public static Result<Filter> Create(
        string name,
        string location,
        decimal? minPrice,
        decimal? maxPrice,
        int? minRooms,
        int? maxRooms,
        decimal? minArea,
        decimal? maxArea,
        string? preferences)
    {
        var filterName = FilterName.Create(name);

        if (filterName.IsFailure)
        {
            return filterName.Error;
        }

        var filterLocation = FilterLocation.Create(location);

        if (filterLocation.IsFailure)
        {
            return filterLocation.Error;
        }

        var filterMinPrice = minPrice is null
            ? null
            : FilterPrice.Create(minPrice.Value);

        if (filterMinPrice is not null && filterMinPrice.IsFailure)
        {
            return filterMinPrice.Error;
        }

        var filterMaxPrice = maxPrice is null
            ? null
            : FilterPrice.Create(maxPrice.Value);

        if (filterMaxPrice is not null && filterMaxPrice.IsFailure)
        {
            return filterMaxPrice.Error;
        }

        var filterMinArea = minArea is null
            ? null
            : FilterArea.Create(minArea.Value);

        if (filterMinArea is not null && filterMinArea.IsFailure)
        {
            return filterMinArea.Error;
        }

        var filterMaxArea = maxArea is null
            ? null
            : FilterArea.Create(maxArea.Value);

        if (filterMaxArea is not null && filterMaxArea.IsFailure)
        {
            return filterMaxArea.Error;
        }

        var filterMinRooms = minRooms is null
            ? null
            : FilterRoomsCount.Create(minRooms.Value);

        if (filterMinRooms is not null && filterMinRooms.IsFailure)
        {
            return filterMinRooms.Error;
        }

        var filterMaxRooms = maxRooms is null
            ? null
            : FilterRoomsCount.Create(maxRooms.Value);

        if (filterMaxRooms is not null && filterMaxRooms.IsFailure)
        {
            return filterMaxRooms.Error;
        }

        var preferencesObject = preferences is null
            ? null
            : Preferences.Create(preferences);

        if (preferencesObject is not null && preferencesObject.IsFailure)
        {
            return preferencesObject.Error;
        }

        return new Filter(
            filterName.Value,
            filterLocation.Value,
            filterMinPrice?.Value,
            filterMaxPrice?.Value,
            filterMinRooms?.Value,
            filterMaxRooms?.Value,
            filterMinArea?.Value,
            filterMaxArea?.Value,
            preferencesObject?.Value);
    }
}