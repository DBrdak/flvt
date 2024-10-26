using System.Reflection;
using Amazon.DynamoDBv2.DocumentModel;
using Flvt.Domain.Filters;
using Flvt.Domain.Subscribers;
using Flvt.Infrastructure.Data.DataModels.Exceptions;
using Flvt.Infrastructure.Data.Extensions;
using Filter = Flvt.Domain.Filters.Filter;

namespace Flvt.Infrastructure.Data.DataModels.Filters;

internal sealed class FilterDataModel : IDataModel<Filter>
{
    public string Id { get; init; }
    public string Name { get; init; }
    public string Location { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public int? MinRooms { get; init; }
    public int? MaxRooms { get; init; }
    public decimal? MinArea { get; init; }
    public decimal? MaxArea { get; init; }
    public string FrequencyName { get; init; }
    public long FrequencyEverySeconds { get; init; }
    public long FrequencyLastUsed { get; init; }
    //TODO Implement Preferences
    public string Tier { get; init; }
    public bool OnlyLast24H { get; init; }
    public IEnumerable<string> FoundAdvertisements {get; init; }
    public IEnumerable<string> RecentlyFoundAdvertisements { get; init; }
    public IEnumerable<string> SeenAdvertisements { get; init; }

    private FilterDataModel(Filter filter)
    {
        Id = filter.Id;
        Name = filter.Name.Value;
        Location = filter.Location.City;
        MinPrice = filter.MinPrice?.Value;
        MaxPrice = filter.MaxPrice?.Value;
        MinRooms = filter.MinRooms?.Value;
        MaxRooms = filter.MaxRooms?.Value;
        MinArea = filter.MinArea?.Value;
        MaxArea = filter.MaxArea?.Value;
        FrequencyName = filter.Frequency.Name;
        FrequencyEverySeconds = filter.Frequency.EverySeconds;
        FrequencyLastUsed = filter.Frequency.LastUsed;
        Tier = filter.Tier.Value;
        OnlyLast24H = filter.OnlyLast24H;
        FoundAdvertisements = filter.FoundAdvertisements;
        RecentlyFoundAdvertisements = filter.RecentlyFoundAdvertisements;
        SeenAdvertisements = filter.SeenAdvertisements;
    }
    private FilterDataModel(Document doc)
    {
        Id = doc[nameof(Id)];
        Name = doc[nameof(Name)];
        Location = doc[nameof(Location)];
        MinPrice = doc[nameof(MinPrice)].AsNullableDecimal();
        MaxPrice = doc[nameof(MaxPrice)].AsNullableDecimal();
        MinRooms = doc[nameof(MinRooms)].AsNullableInt();
        MaxRooms = doc[nameof(MaxRooms)].AsNullableInt();
        MinArea = doc[nameof(MinArea)].AsNullableDecimal();
        MaxArea = doc[nameof(MaxArea)].AsNullableDecimal();
        FrequencyName = doc[nameof(FrequencyName)];
        FrequencyEverySeconds = doc[nameof(FrequencyEverySeconds)].AsLong();
        FrequencyLastUsed = doc[nameof(FrequencyLastUsed)].AsLong();
        Tier = doc[nameof(Tier)];
        OnlyLast24H = doc[nameof(OnlyLast24H)].AsBoolean();
        FoundAdvertisements = doc[nameof(FoundAdvertisements)].AsArrayOfString();
        RecentlyFoundAdvertisements = doc[nameof(RecentlyFoundAdvertisements)].AsArrayOfString();
        SeenAdvertisements = doc[nameof(SeenAdvertisements)].AsArrayOfString();
    }

    public static FilterDataModel FromDomainModel(Filter domainModel) => new (domainModel);

    public static FilterDataModel FromDocument(Document document) => new(document);

    public Type GetDomainModelType() => typeof(Filter);

    public Filter ToDomainModel()
    {
        var name = FilterName.Create(Name).Value;
        var location = Activator.CreateInstance(
                           typeof(FilterLocation),
                           BindingFlags.Instance | BindingFlags.NonPublic,
                           null,
                           [Location],
                           null) ??
                       throw new DataModelConversionException(typeof(string), typeof(FilterLocation));
        var minPrice = Activator.CreateInstance(
                           typeof(FilterPrice),
                           BindingFlags.Instance | BindingFlags.NonPublic,
                           null,
                           [MinPrice],
                           null) ??
                       throw new DataModelConversionException(typeof(decimal), typeof(FilterPrice));
        var maxPrice = Activator.CreateInstance(
                           typeof(FilterPrice),
                           BindingFlags.Instance | BindingFlags.NonPublic,
                           null,
                           [MaxPrice],
                           null) ??
                       throw new DataModelConversionException(typeof(decimal), typeof(FilterPrice));
        var minArea = Activator.CreateInstance(
                           typeof(FilterArea),
                           BindingFlags.Instance | BindingFlags.NonPublic,
                           null,
                           [MinArea],
                           null) ??
                       throw new DataModelConversionException(typeof(decimal), typeof(FilterArea));
        var maxArea = Activator.CreateInstance(
                           typeof(FilterArea),
                           BindingFlags.Instance | BindingFlags.NonPublic,
                           null,
                           [MaxArea],
                           null) ??
                       throw new DataModelConversionException(typeof(decimal), typeof(FilterArea));
        var minRooms = Activator.CreateInstance(
                           typeof(FilterRoomsCount),
                           BindingFlags.Instance | BindingFlags.NonPublic,
                           null,
                           [MinRooms],
                           null) ??
                       throw new DataModelConversionException(typeof(int), typeof(FilterRoomsCount));
        var maxRooms = Activator.CreateInstance(
                           typeof(FilterRoomsCount),
                           BindingFlags.Instance | BindingFlags.NonPublic,
                           null,
                           [MaxRooms],
                           null) ??
                       throw new DataModelConversionException(typeof(int), typeof(FilterRoomsCount));
        var frequency = Activator.CreateInstance(
                            typeof(Frequency),
                            BindingFlags.Instance | BindingFlags.NonPublic,
                            null,
                            [FrequencyName, FrequencyEverySeconds, FrequencyLastUsed],
                            null) ??
                        throw new DataModelConversionException(typeof(string), typeof(Frequency));
        var tier = Activator.CreateInstance(
                       typeof(SubscribtionTier),
                       BindingFlags.Instance | BindingFlags.NonPublic,
                       null,
                       [Tier],
                       null) ??
                   throw new DataModelConversionException(typeof(string), typeof(SubscribtionTier));

        return Activator.CreateInstance(
                   typeof(Filter),
                   BindingFlags.Instance | BindingFlags.NonPublic,
                   null,
                   [
                       Id,
                       name,
                       location,
                       minPrice,
                       maxPrice,
                       minRooms,
                       maxRooms,
                       minArea,
                       maxArea,
                       frequency,
                       tier,
                       null,
                       FoundAdvertisements,
                       RecentlyFoundAdvertisements,
                       SeenAdvertisements,
                       OnlyLast24H
                   ],
                   null) as Filter ??
               throw new DataModelConversionException(typeof(Filter));
    }
}