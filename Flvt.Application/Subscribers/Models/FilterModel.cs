using Flvt.Domain.Filters;

namespace Flvt.Application.Subscribers.Models;

public sealed record FilterModel
{
    public string Id { get; init; }
    public string Name { get; private set; }
    public string Location { get; private set; }
    public decimal? MinPrice { get; private set; }
    public decimal? MaxPrice { get; private set; }
    public int? MinRooms { get; private set; }
    public int? MaxRooms { get; private set; }
    public decimal? MinArea { get; private set; }
    public decimal? MaxArea { get; private set; }
    public string Frequency { get; init; }
    public DateTimeOffset LastUsed { get; init; }
    public DateTimeOffset NextUse { get; init; }
    public string Tier { get; init; }

    private FilterModel(
        string id,
        string name,
        string location,
        decimal? minPrice,
        decimal? maxPrice,
        int? minRooms,
        int? maxRooms,
        decimal? minArea,
        decimal? maxArea,
        string frequency,
        DateTimeOffset lastUsed,
        DateTimeOffset nextUse,
        string tier)
    {
        Id = id;
        Name = name;
        Location = location;
        MinPrice = minPrice;
        MaxPrice = maxPrice;
        MinRooms = minRooms;
        MaxRooms = maxRooms;
        MinArea = minArea;
        MaxArea = maxArea;
        Tier = tier;
        Frequency = frequency;
        LastUsed = lastUsed;
        NextUse = nextUse;
    }

    public static FilterModel FromDomainModel(Filter filter) =>
        new(
            filter.Id,
            filter.Name.Value,
            filter.Location.City,
            filter.MinPrice?.Value,
            filter.MaxPrice?.Value,
            filter.MinRooms?.Value,
            filter.MaxRooms?.Value,
            filter.MinArea?.Value,
            filter.MaxArea?.Value,
            filter.Frequency.Name,
            DateTimeOffset.FromUnixTimeSeconds(filter.Frequency.LastUsed),
            DateTimeOffset.FromUnixTimeSeconds(filter.Frequency.NextUse),
            filter.Tier.Value);
}