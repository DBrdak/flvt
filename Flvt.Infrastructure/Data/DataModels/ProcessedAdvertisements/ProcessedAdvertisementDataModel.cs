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
    public string? AddressCountry { get; init; }
    public string? AddressProvince { get; init; }
    public string? AddressRegion { get; init; }
    public string? AddressCity {get; init; }
    public string? AddressDistrict {get; init; }
    public string? AddressSubdistrict {get; init; }
    public string? AddressStreet {get; init; }
    public string? AddressHouseNumber { get;  init; }
    public string? GeolocationLatitude { get; init; }
    public string? GeolocationLongitude { get; init; }
    public string Description { get; init; }
    public string ContactType { get; init; }
    public decimal PriceAmount { get; init; }
    public string PriceCurrency { get; init; }
    public decimal? DepositAmount { get; init; }
    public decimal? FeeAmount { get; init; }
    public int RoomsValue { get; init; }
    public int FloorSpecific { get; init; }
    public int? FloorTotal { get; init; }
    public decimal AreaAmount { get; init; }
    public string AreaUnit { get; init; }
    public long? AddedAt { get; init; }
    public long? UpdatedAt { get; init; }
    public string? AvailableFrom { get; init; }
    public bool? Pets { get; init; }
    public bool IsFlagged { get; init; }

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
        DepositAmount = original.Deposit;
        FeeAmount = original.Fee;
        RoomsValue = original.RoomsCount;
        FloorSpecific = original.Floor.Specific;
        FloorTotal = original.Floor.Total;
        AreaAmount = original.Area.Amount;
        AreaUnit = original.Area.Unit;
        AddedAt = original.AddedAt?.Ticks;
        UpdatedAt = original.UpdatedAt?.Ticks;
        AvailableFrom = original.AvailableFrom;
        Pets = original.Pets;
        IsFlagged = original.IsFlagged;
    }

    private ProcessedAdvertisementDataModel(Document doc)
    {
        Link = doc.GetProperty(nameof(Link));
        Dedupe = doc.GetProperty(nameof(Dedupe));
        AddressCountry = doc.GetNullableProperty(nameof(AddressCountry))?.AsNullableString();
        AddressProvince = doc.GetNullableProperty(nameof(AddressProvince))?.AsNullableString();
        AddressRegion = doc.GetNullableProperty(nameof(AddressRegion))?.AsNullableString();
        AddressCity = doc.GetNullableProperty(nameof(AddressCity))?.AsNullableString();
        AddressDistrict = doc.GetNullableProperty(nameof(AddressDistrict))?.AsNullableString();
        AddressSubdistrict = doc.GetNullableProperty(nameof(AddressSubdistrict))?.AsNullableString();
        AddressStreet = doc.GetNullableProperty(nameof(AddressStreet))?.AsNullableString();
        AddressHouseNumber = doc.GetNullableProperty(nameof(AddressHouseNumber))?.AsNullableString();
        GeolocationLatitude = doc.GetNullableProperty(nameof(GeolocationLatitude))?.AsNullableString();
        GeolocationLongitude = doc.GetNullableProperty(nameof(GeolocationLongitude))?.AsNullableString();
        Description = doc.GetProperty(nameof(Description));
        ContactType = doc.GetProperty(nameof(ContactType));
        PriceAmount = doc.GetProperty(nameof(PriceAmount)).AsDecimal();
        PriceCurrency = doc.GetProperty(nameof(PriceCurrency));
        DepositAmount = doc.GetNullableProperty(nameof(DepositAmount))?.AsNullableDecimal();
        FeeAmount = doc.GetNullableProperty(nameof(FeeAmount))?.AsNullableDecimal();
        RoomsValue = doc.GetProperty(nameof(RoomsValue)).AsInt();
        FloorSpecific = doc.GetProperty(nameof(FloorSpecific)).AsInt();
        FloorTotal = doc.GetNullableProperty(nameof(FloorTotal))?.AsNullableInt();
        AreaAmount = doc.GetNullableProperty(nameof(AreaAmount))?.AsNullableDecimal() ?? 0;
        AreaUnit = doc.GetProperty(nameof(AreaUnit));
        AddedAt = doc.GetNullableProperty(nameof(AddedAt))?.AsNullableLong();
        UpdatedAt = doc.GetNullableProperty(nameof(UpdatedAt))?.AsNullableLong();
        AvailableFrom = doc.GetNullableProperty(nameof(AvailableFrom))?.AsNullableString();
        Pets = doc.GetNullableProperty(nameof(Pets))?.AsNullableBoolean();
        IsFlagged = doc.GetProperty(nameof(IsFlagged)).AsBoolean();
    }

    public Type GetDomainModelType() => typeof(ProcessedAdvertisement);

    public static ProcessedAdvertisementDataModel FromDomainModel(ProcessedAdvertisement original) =>
        new(original);
    public static ProcessedAdvertisementDataModel FromDocument(Document doc) =>
        new(doc);

    public ProcessedAdvertisement ToDomainModel()
    {
        var price = new Money(PriceAmount, Currency.FromCode(PriceCurrency).Value);
        var rooms = RoomsValue;
        var floor = new Floor(FloorSpecific, FloorTotal);
        var area = new Area(AreaAmount, AreaUnit);
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
            DepositAmount,
            FeeAmount,
            rooms,
            floor,
            area,
            addedAt,
            updatedAt,
            AvailableFrom,
            Pets,
            IsFlagged);
    }
}