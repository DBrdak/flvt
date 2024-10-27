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

    public void ValidateFields()
    {
        if (!Uri.IsWellFormedUriString(Link, UriKind.Absolute))
        {
            throw new ArgumentException("Link is not a valid URL.");
        }

        if (string.IsNullOrWhiteSpace(Description))
        {
            throw new ArgumentException("Description is empty.");
        }

        if (string.IsNullOrWhiteSpace(ContactType))
        {
            throw new ArgumentException("ContactType is empty.");
        }

        if (Price is null)
        {
            throw new ArgumentException("Price is null.");
        }

        if (Price.Amount <= 0)
        {
            throw new ArgumentException("Price is not a positive number.");
        }

        if (string.IsNullOrWhiteSpace(Price.Currency?.Code))
        {
            throw new ArgumentException("Price currency is null.");
        }

        if (Deposit is not null && Deposit.Amount < 0)
        {
            throw new ArgumentException("Deposit is not a positive number.");
        }

        if (Deposit is not null && string.IsNullOrWhiteSpace(Deposit.Currency?.Code))
        {
            throw new ArgumentException("Deposit currency is null.");
        }

        if (Fee is not null && Fee.Amount < 0)
        {
            throw new ArgumentException("Fee is not a positive number.");
        }

        if (Fee is not null && string.IsNullOrWhiteSpace(Fee.Currency?.Code))
        {
            throw new ArgumentException("Fee currency is null.");
        }

        if (Rooms.Value <= 0)
        {
            throw new ArgumentException("Rooms is not a positive number.");
        }

        if (string.IsNullOrWhiteSpace(Rooms.Unit))
        {
            throw new ArgumentException("Rooms unit is empty.");
        }

        if (Area.Amount <= 0)
        {
            throw new ArgumentException("Area is not a positive number.");
        }

        if (string.IsNullOrWhiteSpace(Area.Unit))
        {
            throw new ArgumentException("Area unit is empty.");
        }

        if (Floor.Specific < 0)
        {
            throw new ArgumentException("Floor is not a positive number.");
        }

        if (Floor.Total < 0)
        {
            throw new ArgumentException("Floor is not a positive number.");
        }
    }
}