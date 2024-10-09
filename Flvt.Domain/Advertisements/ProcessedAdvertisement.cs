using Flvt.Domain.Primitives;

namespace Flvt.Domain.Advertisements;

public sealed class ProcessedAdvertisement
{
    public string Link { get; init; }
    public Address Address { get; init; }
    public Coordinates? Location { get; init; }
    public string Description { get; init; }
    public string ContactType { get; init; }
    public Money Price { get; init; }
    public string[] PriceNotes { get; init; }
    public Money? Deposit { get; init; }
    public RoomsCount Rooms { get; init; }
    public Floor Floor { get; init; }
    public Area Area { get; init; }
    public string[] Facilities { get; init; }
    public DateTime? AddedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string? AvailableFrom { get; init; }

    public ProcessedAdvertisement(
        string link,
        Address address,
        Coordinates? location,
        string description,
        string contactType,
        Money price,
        string[] priceNotes,
        Money? deposit,
        RoomsCount rooms,
        Floor floor,
        Area area,
        string[] facilities,
        DateTime? addedAt,
        DateTime? updatedAt,
        string? availableFrom)
    {
        Link = link;
        Address = address;
        Location = location;
        Description = description;
        ContactType = contactType;
        Price = price;
        PriceNotes = priceNotes;
        Deposit = deposit;
        Rooms = rooms;
        Floor = floor;
        Area = area;
        Facilities = facilities;
        AddedAt = addedAt;
        UpdatedAt = updatedAt;
        AvailableFrom = availableFrom;
    }
}