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
    public bool IsPrivate { get; set; }
    public Money Price { get; init; }
    public decimal? Deposit { get; init; }
    public decimal? Fee { get; init; }
    public int RoomsCount { get; init; }
    public Area Area { get; init; }
    public DateTime? AddedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string? AvailableFrom { get; init; }
    public bool? Pets { get; init; }
    public bool IsFlagged { get; private set; }
    public long NextOutdateCheck { get; private set; }

    public ProcessedAdvertisement(
        string link,
        Address address,
        Coordinates? geolocation,
        string description,
        bool isPrivate,
        Money price,
        decimal? deposit,
        decimal? fee,
        int roomsCount,
        Area area,
        DateTime? addedAt,
        DateTime? updatedAt,
        string? availableFrom,
        bool? pets,
        long nextOutdateCheck,
        bool isFlagged = false)
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
        NextOutdateCheck = nextOutdateCheck;
        IsFlagged = isFlagged;
        Dedupe =
            $"{address?.City}-{address?.District}-{address?.Street}-{isPrivate}-{roomsCount}-{area.Amount}-{price.Amount}-{pets}-{deposit}-{geolocation?.Latitude}-{geolocation?.Longitude}"
                .ToLower();
    }

    public void Flag() => IsFlagged = true;

    public void CheckedForOutdate() => NextOutdateCheck = DateTimeOffset.UtcNow.AddDays(2).ToUnixTimeSeconds();

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

        if (RoomsCount <= 0)
        {
            throw new ArgumentException("Rooms is not a positive number.");
        }

        if (Area.Amount <= 0)
        {
            throw new ArgumentException("Area is not a positive number.");
        }

        if (string.IsNullOrWhiteSpace(Area.Unit))
        {
            throw new ArgumentException("Area unit is empty.");
        }
    }
}