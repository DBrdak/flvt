﻿using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.Subscribers;

namespace Flvt.Domain.Filters;

public sealed record Filter
{
    public string Id { get; init; }
    public FilterName Name { get; private set; }
    public FilterLocation Location { get; private set; }
    public FilterPrice? MinPrice { get; private set; }
    public FilterPrice? MaxPrice { get; private set; }
    public FilterRoomsCount? MinRooms { get; private set; }
    public FilterRoomsCount? MaxRooms { get; private set; }
    public FilterArea? MinArea { get; private set; }
    public FilterArea? MaxArea { get; private set; }
    public Frequency Frequency { get; init; }
    public SubscribtionTier Tier { get; init; }
    public bool OnlyLast24H { get; init; }
    private List<string> _foundAdvertisements;
    public IReadOnlyList<string> FoundAdvertisements => _foundAdvertisements;
    private List<string> _recentlyFoundAdvertisements;
    public IReadOnlyList<string> RecentlyFoundAdvertisements => _recentlyFoundAdvertisements;
    private List<string> _seenAdvertisements;
    public IReadOnlyList<string> SeenAdvertisements => _seenAdvertisements;
    private List<string> _followedAdvertisements;
    public IReadOnlyCollection<string> FollowedAdvertisements => _followedAdvertisements;
    public string? AdvertisementsFilePath { get; private set; }
    public bool ShouldLaunch => Frequency.NextUse <= DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    private Filter(
        string id,
        FilterName name,
        FilterLocation location,
        FilterPrice? minPrice,
        FilterPrice? maxPrice,
        FilterRoomsCount? minRooms,
        FilterRoomsCount? maxRooms,
        FilterArea? minArea,
        FilterArea? maxArea,
        Frequency frequency,
        SubscribtionTier tier,
        List<string> foundAdvertisements,
        List<string> recentlyFoundAdvertisements,
        List<string> seenAdvertisements,
        List<string> followedAdvertisements,
        string? advertisementsFilePath,
        bool onlyLast24H = false)
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
        Frequency = frequency;
        _foundAdvertisements = foundAdvertisements;
        _recentlyFoundAdvertisements = recentlyFoundAdvertisements;
        _seenAdvertisements = seenAdvertisements;
        _followedAdvertisements = followedAdvertisements;
        AdvertisementsFilePath = advertisementsFilePath;
        Tier = tier;
        OnlyLast24H = onlyLast24H;
    }

    public static Filter CreateForInternalScan(FilterLocation location) =>
        new (
            Ulid.NewUlid().ToString(),
            FilterName.Create($"Scan-{DateTime.UtcNow:yyyy/MM//dd HH:mm:ss.fff}").Value,
            location,
            null,
            null,
            null,
            null,
            null,
            null,
            Frequency.Daily,
            SubscribtionTier.Basic,
            [],
            [],
            [],
            [],
            null,
            true);

    internal static Result<Filter> Create(
        string name,
        string location,
        decimal? minPrice,
        decimal? maxPrice,
        int? minRooms,
        int? maxRooms,
        decimal? minArea,
        decimal? maxArea,
        Frequency frequency,
        SubscribtionTier tier)
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

        return new Filter(
            Ulid.NewUlid().ToString(),
            filterName.Value,
            filterLocation.Value,
            filterMinPrice?.Value,
            filterMaxPrice?.Value,
            filterMinRooms?.Value,
            filterMaxRooms?.Value,
            filterMinArea?.Value,
            filterMaxArea?.Value,
            frequency,
            tier,
            [],
            [],
            [],
            [],
            null);
    }

    public void NewAdvertisementsFound(List<string> advertisements, string advertisementsFilePath)
    {
        _recentlyFoundAdvertisements = advertisements
            .Where(ad => _foundAdvertisements.All(foundAd => foundAd != ad))
            .ToList();

        _foundAdvertisements = advertisements;

        _seenAdvertisements = _seenAdvertisements
            .Where(seenAd => _foundAdvertisements.Contains(seenAd))
            .ToList();

        _followedAdvertisements = _followedAdvertisements
            .Where(followedAd => _foundAdvertisements.Contains(followedAd))
            .ToList();

        Frequency.Used();
        AdvertisementsFilePath = advertisementsFilePath;
    }

    public void MarkAsSeen(string advertisement) => _seenAdvertisements.Add(advertisement);

    public void Follow(string advertisement) => _followedAdvertisements.Add(advertisement);
}