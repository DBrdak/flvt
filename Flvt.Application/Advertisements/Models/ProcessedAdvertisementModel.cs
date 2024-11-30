using Flvt.Domain.Filters;
using Flvt.Domain.Photos;
using Flvt.Domain.Primitives.Advertisements;
using Flvt.Domain.Primitives.Money;
using Flvt.Domain.ProcessedAdvertisements;

namespace Flvt.Application.Advertisements.Models;

public sealed record ProcessedAdvertisementModel
{
    public string Link { get; init; }
    public Address Address { get; init; }
    public Coordinates? Geolocation { get; init; }
    public string Description { get; init; }
    public bool IsPrivate { get; init; }
    public Money Price { get; init; }
    public Money? Deposit { get; init; }
    public Money? Fee { get; init; }
    public int RoomsCount { get; init; }
    public Area Area { get; init; }
    public DateTime? AddedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string? AvailableFrom { get; init; }
    public bool? Pets { get; init; }
    public IEnumerable<string> Photos { get; init; }
    public bool IsFlagged { get; init; }
    public bool WasSeen { get; init; }
    public bool IsNew { get; init; }
    public bool IsFollowed { get; init; }

    private ProcessedAdvertisementModel(
        string link,
        Address address,
        Coordinates? geolocation,
        string description,
        bool isPrivate,
        Money price,
        Money? deposit,
        Money? fee,
        int roomsCount,
        Area area,
        DateTime? addedAt,
        DateTime? updatedAt,
        string? availableFrom,
        bool? pets,
        IEnumerable<string> photos,
        bool isFlagged,
        bool wasSeen,
        bool isNew,
        bool isFollowed)
    {
        Link = link;
        Address = address;
        Geolocation = geolocation;
        Description = description;
        IsPrivate = isPrivate;
        Price = price;
        Deposit = deposit;
        Fee = fee;
        RoomsCount = roomsCount;
        Area = area;
        AddedAt = addedAt;
        UpdatedAt = updatedAt;
        AvailableFrom = availableFrom;
        Pets = pets;
        Photos = photos;
        IsFlagged = isFlagged;
        WasSeen = wasSeen;
        IsNew = isNew;
        IsFollowed = isFollowed;
    }

    internal static ProcessedAdvertisementModel FromDomainModel(
        ProcessedAdvertisement processedAdvertisement,
        IEnumerable<string> photos,
        bool wasSeen,
        bool isNew,
        bool isFollowed) =>
        new(
            processedAdvertisement.Link,
            processedAdvertisement.Address,
            processedAdvertisement.Geolocation,
            processedAdvertisement.Description,
            processedAdvertisement.IsPrivate,
            processedAdvertisement.Price,
            processedAdvertisement.Deposit is var deposit && deposit is not null ?
                new Money(deposit!.Value, processedAdvertisement.Price.Currency) : 
                null,
            processedAdvertisement.Fee is var fee && fee is not null ?
                new Money(fee!.Value, processedAdvertisement.Price.Currency) :
                null,
            processedAdvertisement.RoomsCount,
            processedAdvertisement.Area,
            processedAdvertisement.AddedAt,
            processedAdvertisement.UpdatedAt,
            processedAdvertisement.AvailableFrom,
            processedAdvertisement.Pets,
            photos,
            processedAdvertisement.IsFlagged,
            wasSeen, 
            isNew, 
            isFollowed);

    internal static List<ProcessedAdvertisementModel> FromFilter(
        Filter filter,
        List<ProcessedAdvertisement> advertisements,
        List<AdvertisementPhotos> photos)
    {
        List<ProcessedAdvertisementModel> advertisementsModels = [];
        var seenAdvertisements =
            advertisements
                .Where(ad => filter.SeenAdvertisements.Any(seenAd => seenAd == ad.Link))
                .ToList();
        var newAdvertisements =
            advertisements
                .Where(ad => filter.RecentlyFoundAdvertisements.Any(newAd => newAd == ad.Link))
                .ToList();
        var followedAdvertisements =
            advertisements
                .Where(ad => filter.FollowedAdvertisements.Any(followedAd => followedAd == ad.Link))
                .ToList();

        advertisementsModels.AddRange(
            from advertisement in advertisements
            let wasSeen = seenAdvertisements.Any(ad => ad.Link == advertisement.Link)
            let isNew = newAdvertisements.Any(ad => ad.Link == advertisement.Link)
            let isFollowed = followedAdvertisements.Any(ad => ad.Link == advertisement.Link)
            let advertisementPhotos = photos.FirstOrDefault(photo => photo.AdvertisementLink == advertisement.Link)?.Links
            select FromDomainModel(advertisement, advertisementPhotos, wasSeen, isNew, isFollowed));

        return advertisementsModels;
    }
}