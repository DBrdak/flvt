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
    public string ContactType { get; init; }
    public Money Price { get; init; }
    public Money? Deposit { get; init; }
    public RoomsCount Rooms { get; init; }
    public Floor Floor { get; init; }
    public Area Area { get; init; }
    public string[] Facilities { get; init; }
    public DateTime? AddedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string? AvailableFrom { get; init; }
    public bool? Pets { get; init; }
    public IEnumerable<string> Photos { get; init; }
    public bool IsFlagged { get; private set; }
    public bool IsSeen { get; init; }
    public bool IsNew { get; init; }

    private ProcessedAdvertisementModel(
        string link,
        Address address,
        Coordinates? geolocation,
        string description,
        string contactType,
        Money price,
        Money? deposit,
        RoomsCount rooms,
        Floor floor,
        Area area,
        string[] facilities,
        DateTime? addedAt,
        DateTime? updatedAt,
        string? availableFrom,
        bool? pets,
        IEnumerable<string> photos,
        bool isFlagged)
    {
        Link = link;
        Address = address;
        Geolocation = geolocation;
        Description = description;
        ContactType = contactType;
        Price = price;
        Deposit = deposit;
        Rooms = rooms;
        Floor = floor;
        Area = area;
        Facilities = facilities;
        AddedAt = addedAt;
        UpdatedAt = updatedAt;
        AvailableFrom = availableFrom;
        Pets = pets;
        Photos = photos;
        IsFlagged = isFlagged;
    }

    internal static ProcessedAdvertisementModel FromDomainModel(ProcessedAdvertisement processedAdvertisement, AdvertisementPhotos photos) =>
        new(
            processedAdvertisement.Link,
            processedAdvertisement.Address,
            processedAdvertisement.Geolocation,
            processedAdvertisement.Description,
            processedAdvertisement.ContactType,
            processedAdvertisement.Price,
            processedAdvertisement.Deposit,
            processedAdvertisement.Rooms,
            processedAdvertisement.Floor,
            processedAdvertisement.Area,
            processedAdvertisement.Facilities,
            processedAdvertisement.AddedAt,
            processedAdvertisement.UpdatedAt,
            processedAdvertisement.AvailableFrom,
            processedAdvertisement.Pets,
            photos.Links,
            processedAdvertisement.IsFlagged);
}