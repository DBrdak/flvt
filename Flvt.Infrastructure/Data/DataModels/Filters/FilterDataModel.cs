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
    public string Tier { get; init; }
    public bool OnlyLast24H { get; init; }
    public IEnumerable<string> FoundAdvertisements {get; init; }
    public IEnumerable<string> RecentlyFoundAdvertisements { get; init; }
    public IEnumerable<string> SeenAdvertisements { get; init; }
    public IEnumerable<string> FollowedAdvertisements { get; init; }
    public string? AdvertisementsFilePath { get; init; }
    public string SubscriberEmail { get; init; }

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
        FollowedAdvertisements = filter.FollowedAdvertisements;
        AdvertisementsFilePath = filter.AdvertisementsFilePath;
        SubscriberEmail = filter.SubscriberEmail;
    }
    private FilterDataModel(Document doc)
    {
        Id = doc.GetProperty(nameof(Id));
        Name = doc.GetProperty(nameof(Name));
        Location = doc.GetProperty(nameof(Location));
        MinPrice = doc.GetNullableProperty(nameof(MinPrice))?.AsNullableDecimal();
        MaxPrice = doc.GetNullableProperty(nameof(MaxPrice))?.AsNullableDecimal();
        MinRooms = doc.GetNullableProperty(nameof(MinRooms))?.AsNullableInt();
        MaxRooms = doc.GetNullableProperty(nameof(MaxRooms))?.AsNullableInt();
        MinArea = doc.GetNullableProperty(nameof(MinArea))?.AsNullableDecimal();
        MaxArea = doc.GetNullableProperty(nameof(MaxArea))?.AsNullableDecimal();
        FrequencyName = doc.GetProperty(nameof(FrequencyName));
        FrequencyEverySeconds = doc.GetProperty(nameof(FrequencyEverySeconds)).AsLong();
        FrequencyLastUsed = doc.GetProperty(nameof(FrequencyLastUsed)).AsLong();
        Tier = doc.GetProperty(nameof(Tier));
        OnlyLast24H = doc.GetProperty(nameof(OnlyLast24H)).AsBoolean();
        FoundAdvertisements = doc.GetProperty(nameof(FoundAdvertisements)).AsArrayOfString();
        RecentlyFoundAdvertisements = doc.GetProperty(nameof(RecentlyFoundAdvertisements)).AsArrayOfString();
        SeenAdvertisements = doc.GetProperty(nameof(SeenAdvertisements)).AsArrayOfString();
        FollowedAdvertisements = doc.GetProperty(nameof(FollowedAdvertisements)).AsArrayOfString();
        AdvertisementsFilePath = doc.GetNullableProperty(nameof(AdvertisementsFilePath))?.AsNullableString();
        SubscriberEmail = doc.GetProperty(nameof(SubscriberEmail));
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
        var minPrice = MinPrice is null ? null : Activator.CreateInstance(
                                                    typeof(FilterPrice),
                                                    BindingFlags.Instance | BindingFlags.NonPublic,
                                                    null,
                                                    [MinPrice.Value],
                                                    null) ??
                                                throw new DataModelConversionException(typeof(decimal), typeof(FilterPrice));
        var maxPrice = MaxPrice is null ? null : Activator.CreateInstance(
                                                     typeof(FilterPrice),
                                                     BindingFlags.Instance | BindingFlags.NonPublic,
                                                     null,
                                                     [MaxPrice.Value],
                                                     null) ??
                                                 throw new DataModelConversionException(typeof(decimal), typeof(FilterPrice));
        var minArea = MinArea is null ? null : Activator.CreateInstance(
                                                   typeof(FilterArea),
                                                   BindingFlags.Instance | BindingFlags.NonPublic,
                                                   null,
                                                   [MinArea.Value],
                                                   null) ??
                                               throw new DataModelConversionException(typeof(decimal), typeof(FilterArea));
        var maxArea = MaxArea is null ? null : Activator.CreateInstance(
                                                   typeof(FilterArea),
                                                   BindingFlags.Instance | BindingFlags.NonPublic,
                                                   null,
                                                   [MaxArea.Value],
                                                   null) ??
                                               throw new DataModelConversionException(typeof(decimal), typeof(FilterArea));
        var minRooms = MinRooms is null ? null : Activator.CreateInstance(
                                                     typeof(FilterRoomsCount),
                                                     BindingFlags.Instance | BindingFlags.NonPublic,
                                                     null,
                                                     [MinRooms.Value],
                                                     null) ??
                                                 throw new DataModelConversionException(typeof(int), typeof(FilterRoomsCount));
        var maxRooms = MaxRooms is null ? null : Activator.CreateInstance(
                                                     typeof(FilterRoomsCount),
                                                     BindingFlags.Instance | BindingFlags.NonPublic,
                                                     null,
                                                     [MaxRooms.Value],
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
                                     FoundAdvertisements.ToList(),
                                     RecentlyFoundAdvertisements.ToList(),
                                     SeenAdvertisements.ToList(),
                                     FollowedAdvertisements.ToList(),
                                     AdvertisementsFilePath,
                                     SubscriberEmail,
                                     OnlyLast24H
                                 ],
                                 null) as Filter ??
                             throw new DataModelConversionException(typeof(Filter));
    }
}