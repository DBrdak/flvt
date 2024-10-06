using Flvt.Domain.Advertisements.Errors;
using Flvt.Domain.Primitives;

namespace Flvt.Domain.Advertisements;

public sealed class ScrapedAdvertisement
{
    public string Link { get; init; }
    public string Location { get; init; }
    public string Description { get; init; }
    public string ContactType { get; init; }
    public Money Price { get; init; }
    public RoomsCount Rooms { get; init; }
    public string Floor { get; init; }
    public Area Area { get; init; }
    public DateTime? AddedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }

    public ScrapedAdvertisement(
        string link,
        string location,
        string description,
        string contactType,
        Money price,
        RoomsCount rooms,
        string floor,
        Area area,
        DateTime? addedAt,
        DateTime? updatedAt)
    {
        Link = link;
        Location = location;
        Description = description;
        ContactType = contactType;
        Price = price;
        Rooms = rooms;
        Floor = floor;
        Area = area;
        AddedAt = addedAt;
        UpdatedAt = updatedAt;
    }

    public static Result<ScrapedAdvertisement> CreateFromScrapedContent(
        string advertisementLink,
        string? location,
        string? description,
        string? contactType,
        string? price,
        string? currency,
        string? roomsCount,
        string? roomsUnit,
        string? areaValue,
        string? areaUnit,
        string? floor,
        string? addedAt,
        string? updatedAt)
    {

        return advertisementLink switch
        {
            _ when location is null => ScrapedAdvertisementErrors.LocationNotFound,
            _ when description is null => ScrapedAdvertisementErrors.DescriptionNotFound,
            _ when contactType is null => ScrapedAdvertisementErrors.ContactTypeNotFound,
            _ when price is null => ScrapedAdvertisementErrors.PriceNotFound,
            _ when currency is null => ScrapedAdvertisementErrors.CurrencyNotFound,
            _ when roomsCount is null => ScrapedAdvertisementErrors.RoomsNotFound,
            _ when roomsUnit is null => ScrapedAdvertisementErrors.RoomsNotFound,
            _ when areaValue is null => ScrapedAdvertisementErrors.AreaNotFound,
            _ when areaUnit is null => ScrapedAdvertisementErrors.AreaNotFound,
            _ when Currency.FromCode(currency!) is { IsFailure: true } currencyResult => currencyResult.Error,
            _ => new ScrapedAdvertisement(
                advertisementLink,
                location!,
                description!,
                contactType!,
                new Money(int.Parse(price!), Currency.FromCode(currency).Value),
                new RoomsCount(int.Parse(roomsCount!), roomsUnit),
                floor ?? "",
                new Area(decimal.Parse(areaValue!), areaUnit),
                addedAt is null ? null : DateTime.Parse(addedAt),
                updatedAt is null ? null : DateTime.Parse(updatedAt))
        };
    }
}