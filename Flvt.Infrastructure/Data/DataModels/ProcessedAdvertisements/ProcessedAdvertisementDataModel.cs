using Amazon.DynamoDBv2.DocumentModel;
using Flvt.Domain.Primitives.Advertisements;
using Flvt.Domain.Primitives.Money;
using Flvt.Domain.ProcessedAdvertisements;
using Flvt.Infrastructure.Data.Extensions;

namespace Flvt.Infrastructure.Data.DataModels.ProcessedAdvertisements;

internal sealed class ProcessedAdvertisementDataModel : IDataModel<ProcessedAdvertisement>
{
    public string Link { get; init; }
    public string Dedupe { get; init; }
    public string AddressCountry { get; init; }
    public string AddressProvince { get; init; }
    public string AddressRegion { get; init; }
    public string AddressCity {get; init; }
    public string AddressDistrict {get; init; }
    public string AddressSubdistrict {get; init; }
    public string AddressStreet {get; init; }
    public string AddressHouseNumber { get;  init; }
    public string? GeolocationLatitude { get; init; }
    public string? GeolocationLongitude { get; init; }
    public string Description { get; init; }
    public string ContactType { get; init; }
    public decimal PriceAmount { get; init; }
    public string PriceCurrency { get; init; }
    public decimal? DepositAmount { get; init; }
    public string? DepositCurrency { get; init; }
    public int RoomsValue { get; init; }
    public string RoomsUnit { get; init; }
    public int FloorSpecific { get; init; }
    public int? FloorTotal { get; init; }
    public decimal AreaValue { get; init; }
    public string AreaUnit { get; init; }
    public string[] Facilities { get; init; }
    public long? AddedAt { get; init; }
    public long? UpdatedAt { get; init; }
    public string? AvailableFrom { get; init; }
    public bool? Pets { get; init; }
    public IEnumerable<string> Photos { get; init; }
    public bool IsFlagged { get; private set; }

    private ProcessedAdvertisementDataModel(ProcessedAdvertisement original)
    {
        Link = original.Link;
        Dedupe = original.Dedupe;
        AddressCountry = original.Address.Country;
        AddressProvince = original.Address.Province;
        AddressRegion = original.Address.Region;
        AddressCity = original.Address.City;
        AddressDistrict = original.Address.District;
        AddressSubdistrict = original.Address.Subdistrict;
        AddressStreet = original.Address.Street;
        AddressHouseNumber = original.Address.HouseNumber;
        GeolocationLatitude = original.Geolocation?.Latitude;
        GeolocationLongitude = original.Geolocation?.Longitude;
        Description = original.Description;
        ContactType = original.ContactType;
        PriceAmount = original.Price.Amount;
        PriceCurrency = original.Price.Currency.Code;
        DepositAmount = original.Deposit?.Amount;
        DepositCurrency = original.Deposit?.Currency.Code;
        RoomsValue = original.Rooms.Value;
        RoomsUnit = original.Rooms.Unit;
        FloorSpecific = original.Floor.Specific;
        FloorTotal = original.Floor.Total;
        AreaValue = original.Area.Value;
        AreaUnit = original.Area.Unit;
        Facilities = original.Facilities;
        AddedAt = original.AddedAt?.Ticks;
        UpdatedAt = original.UpdatedAt?.Ticks;
        AvailableFrom = original.AvailableFrom;
        Pets = original.Pets;
        Photos = original.Photos;
        IsFlagged = original.IsFlagged;
    }

    private ProcessedAdvertisementDataModel(Document doc)
    {
        Link = doc[nameof(Link)];
        Dedupe = doc[nameof(Dedupe)];
        AddressCountry = doc[nameof(AddressCountry)];
        AddressProvince = doc[nameof(AddressProvince)];
        AddressRegion = doc[nameof(AddressRegion)];
        AddressCity = doc[nameof(AddressCity)];
        AddressDistrict = doc[nameof(AddressDistrict)];
        AddressSubdistrict = doc[nameof(AddressSubdistrict)];
        AddressStreet = doc[nameof(AddressStreet)];
        AddressHouseNumber = doc[nameof(AddressHouseNumber)];
        GeolocationLatitude = doc[nameof(GeolocationLatitude)];
        GeolocationLongitude = doc[nameof(GeolocationLongitude)];
        Description = doc[nameof(Description)];
        ContactType = doc[nameof(ContactType)];
        PriceAmount = doc[nameof(PriceAmount)].AsDecimal();
        PriceCurrency = doc[nameof(PriceCurrency)];
        DepositAmount = doc[nameof(DepositAmount)].AsNullableDecimal();
        DepositCurrency = doc[nameof(DepositCurrency)].AsNullableString();
        RoomsValue = doc[nameof(RoomsValue)].AsInt();
        RoomsUnit = doc[nameof(RoomsUnit)];
        FloorSpecific = doc[nameof(FloorSpecific)].AsInt();
        FloorTotal = doc[nameof(FloorTotal)].AsNullableInt();
        AreaValue = doc[nameof(AreaValue)].AsDecimal();
        AreaUnit = doc[nameof(AreaUnit)];
        Facilities = doc[nameof(Facilities)].AsArrayOfString();
        AddedAt = doc[nameof(AddedAt)].AsLong();
        UpdatedAt = doc[nameof(UpdatedAt)].AsLong();
        AvailableFrom = doc[nameof(AvailableFrom)];
        Pets = doc[nameof(Pets)].AsBoolean();
        Photos = doc[nameof(Photos)].AsArrayOfString();
        IsFlagged = doc[nameof(IsFlagged)].AsBoolean();
    }

    public Type GetDomainModelType() => typeof(ProcessedAdvertisement);

    public static ProcessedAdvertisementDataModel FromDomainModel(ProcessedAdvertisement original) =>
        new(original);
    public static ProcessedAdvertisementDataModel FromDocument(Document doc) =>
        new(doc);

    public ProcessedAdvertisement ToDomainModel()
    {
        var price = new Money(PriceAmount, Currency.FromCode(PriceCurrency).Value);
        var deposit = DepositAmount.HasValue
            ? new Money(DepositAmount.Value, Currency.FromCode(DepositCurrency!).Value)
            : null;
        var rooms = new RoomsCount(RoomsValue, RoomsUnit);
        var floor = new Floor(FloorSpecific, FloorTotal);
        var area = new Area(AreaValue, AreaUnit);
        var address = new Address(
            AddressCountry,
            AddressProvince,
            AddressRegion,
            AddressCity,
            AddressDistrict,
            AddressSubdistrict,
            AddressStreet,
            AddressHouseNumber);
        var geolocation = GeolocationLatitude is not null && GeolocationLongitude is not null ?
            new Coordinates(GeolocationLatitude, GeolocationLongitude) :
            null;
        DateTime? addedAt = AddedAt.HasValue ? new DateTime(AddedAt.Value) : null;
        DateTime? updatedAt = UpdatedAt.HasValue ? new DateTime(UpdatedAt.Value) : null;

        return new ProcessedAdvertisement(
            Link,
            address,
            geolocation,
            Description,
            ContactType,
            price,
            deposit,
            rooms,
            floor,
            area,
            Facilities,
            addedAt,
            updatedAt,
            AvailableFrom,
            Pets,
            Photos,
            IsFlagged);
    }
}