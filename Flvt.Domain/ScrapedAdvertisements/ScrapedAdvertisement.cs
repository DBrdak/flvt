using Flvt.Domain.Extensions;
using Flvt.Domain.Primitives.Advertisements;
using Flvt.Domain.Primitives.Money;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ScrapedAdvertisements.Errors;

namespace Flvt.Domain.ScrapedAdvertisements;

public sealed class ScrapedAdvertisement
{
    public string Link { get; init; }
    public string Location { get; init; }
    public string Description { get; init; }
    public string ContactType { get; init; }
    public Money Price { get; init; }
    public RoomsCount Rooms { get; init; }
    public Floor? Floor { get; init; }
    public Area Area { get; init; }
    public DateTime? AddedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public DateTime? LastScrapedAt { get; init; }
    public IEnumerable<string> Photos { get; init; }
    public bool IsProcessed { get; private set; }


    public ScrapedAdvertisement(
        string link,
        string location,
        string description,
        string contactType,
        Money price,
        RoomsCount rooms,
        Floor? floor,
        Area area,
        DateTime? addedAt,
        DateTime? updatedAt,
        DateTime? lastScrapedAt,
        IEnumerable<string> photos,
        bool isProcessed = false)
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
        LastScrapedAt = lastScrapedAt;
        Photos = photos;
        IsProcessed = isProcessed;
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
        string? specificFloor,
        string? totalFloors,
        string? addedAt,
        string? updatedAt,
        IEnumerable<string>? photos)
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
                specificFloor is null || totalFloors is null ?
                    null :
                    new(specificFloor, totalFloors),
                new Area(decimal.Parse(areaValue!), areaUnit),
                string.IsNullOrWhiteSpace(addedAt) ? null : DateParser.ParseDate(addedAt),
                string.IsNullOrWhiteSpace(updatedAt) ? null : DateParser.ParseDate(updatedAt),
                DateTime.UtcNow,
                photos ?? [])
        };
    }

    private void Process() => IsProcessed = true;
}