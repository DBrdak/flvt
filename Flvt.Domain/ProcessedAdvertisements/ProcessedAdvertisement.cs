using Flvt.Domain.Primitives.Advertisements;
using Flvt.Domain.Primitives.Money;

namespace Flvt.Domain.ProcessedAdvertisements;

public sealed class ProcessedAdvertisement
{
    public string Link { get; init; }
    public string Dedupe { get; init; }
    public Address Address { get; init; }
    public Coordinates? Geolocation { get; init; }
    public string Description { get; init; }
    public string ContactType { get; init; }
    public Money Price { get; init; }
    public Money? Deposit { get; init; }
    public Money? Fee { get; init; }
    public RoomsCount Rooms { get; init; }
    public Floor Floor { get; init; }
    public Area Area { get; init; }
    public string[] Facilities { get; init; }
    public DateTime? AddedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string? AvailableFrom { get; init; }
    public bool? Pets { get; init; }
    public bool IsFlagged { get; private set; }

    public ProcessedAdvertisement(
        string link,
        Address address,
        Coordinates? geolocation,
        string description,
        string contactType,
        Money price,
        Money? deposit,
        Money? fee,
        RoomsCount rooms,
        Floor floor,
        Area area,
        string[] facilities,
        DateTime? addedAt,
        DateTime? updatedAt,
        string? availableFrom,
        bool? pets,
        bool isFlagged = false)
    {
        Link = link;
        Address = address;
        Geolocation = geolocation;
        Description = description;
        ContactType = contactType;
        Price = price;
        Fee = fee;
        Deposit = deposit;
        Rooms = rooms;
        Floor = floor;
        Area = area;
        Facilities = facilities;
        AddedAt = addedAt;
        UpdatedAt = updatedAt;
        AvailableFrom = availableFrom;
        Pets = pets;
        IsFlagged = isFlagged;
        Dedupe =
            $"{Address?.City}-{Address?.District}-{Address?.Street}-{ContactType}-{Rooms.Value}-{Area.Amount}-{Floor.Specific}-{Floor.Total}"
                .ToLower();
    }

    public void Flag() => IsFlagged = true;
}