using System.Collections;
using Flvt.Domain.Filters;
using Flvt.Domain.Primitives.Advertisements;
using Flvt.Domain.Primitives.Money;
using Flvt.Domain.Primitives.Responses;

namespace Flvt.Domain.Subscribers;

public sealed class Subscriber
{
    public Email Email { get; init; }
    public SubscribtionTier Tier { get; init; }
    public Country Country { get; init; }
    private readonly List<string> _filtersIds;
    public IReadOnlyList<string> Filters => _filtersIds;

    private Subscriber(
        Email email, 
        Country country, 
        SubscribtionTier tier,
        List<string> filtersIds)
    {
        Email = email;
        Country = country;
        Tier = tier;
        _filtersIds = filtersIds;
    }

    public static Result<Subscriber> Register(string email, string countryCode)
    {
        var emailResult = Email.Create(email);

        if (emailResult.IsFailure)
        {
            return emailResult.Error;
        }

        var countryResult = Country.Create(countryCode);

        if (countryResult.IsFailure)
        {
            return countryResult.Error;
        }

        return new Subscriber(emailResult.Value, countryResult.Value, SubscribtionTier.Basic, []);
    }

    public Result<Filter> AddBasicFilter(
        string name,
        string city,
        decimal minPrice,
        decimal maxPrice,
        int minRooms,
        int maxRooms,
        decimal minArea,
        decimal maxArea)
    {
        if (Tier == SubscribtionTier.Basic && _filtersIds.Count >= 1)
        {
            return SubscriberErrors.ActionNotAllowedForBasicSubscribers;
        }

        var filterCreateResult = FilterFactory.CreateBasicFilter(
            name,
            city,
            minPrice,
            maxPrice,
            minRooms,
            maxRooms,
            minArea,
            maxArea);

        if (filterCreateResult.IsFailure)
        {
            return filterCreateResult.Error;
        }

        var filter = filterCreateResult.Value;
        
        _filtersIds.Add(filter.Id);

        return Result.Success(filter);
    }
}