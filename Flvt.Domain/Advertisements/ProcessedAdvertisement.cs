using Flvt.Domain.Primitives;

namespace Flvt.Domain.Advertisements;

public sealed class ProcessedAdvertisement
{
    public string Link { get; init; }
    public string Address { get; init; }
    public Coordinates? Location { get; init; }
    public string Description { get; init; }
    public string ContactType { get; init; }
    public Money Price { get; init; }
    public string PriceNotes { get; init; }
    public Money? Deposit { get; init; }
    public RoomsCount Rooms { get; init; }
    public string Floor { get; init; }
    public Area Area { get; init; }
    public string[] Facilities { get; init; }
    public DateTime? AddedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string? AvailableFrom { get; init; }

    public ProcessedAdvertisement(
        ScrapedAdvertisement scrapedAdvertisement,
        string description,
        decimal price,
        string priceNotes,
        decimal? deposit,
        string? availableFrom,
        string[] facilities,
        Coordinates? coordinates)
    {
        Link = scrapedAdvertisement.Link;
        Address = scrapedAdvertisement.Location;
        Location = coordinates;
        Description = description;
        ContactType = scrapedAdvertisement.ContactType;
        Price = new Money(price, scrapedAdvertisement.Price.Currency);
        PriceNotes = priceNotes;
        Deposit = deposit is null ? null : new Money(deposit.Value, scrapedAdvertisement.Price.Currency);
        Rooms = scrapedAdvertisement.Rooms;
        Floor = scrapedAdvertisement.Floor;
        Area = scrapedAdvertisement.Area;
        AddedAt = scrapedAdvertisement.AddedAt;
        UpdatedAt = scrapedAdvertisement.UpdatedAt;
        AvailableFrom = availableFrom;
        Facilities = facilities;
    }
}