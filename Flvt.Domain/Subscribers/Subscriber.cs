using Flvt.Domain.Primitives.Advertisements;
using Flvt.Domain.Primitives.Filters;
using Flvt.Domain.Primitives.Money;
using Flvt.Domain.Primitives.Responses;

namespace Flvt.Domain.Subscribers;

public sealed class Subscriber
{
    public Email Email { get; init; }
    public SubscribtionTier Tier { get; init; }
    public List<Filter> Filters { get; init; }
    public Country Country { get; init; }

    private Subscriber(Email email, List<Filter> filters, Country country, SubscribtionTier tier)
    {
        Email = email;
        Filters = filters;
        Country = country;
        Tier = tier;
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

        return new Subscriber(emailResult.Value, new List<Filter>(), countryResult.Value, SubscribtionTier.Basic);
    }

    public void AddBasicFilter(
        string city,
        Money minPrice,
        Money maxPrice,
        int minRooms,
        int maxRooms,
        int minFloor,
        int maxFloor,
        decimal minArea,
        decimal maxArea)
    {

    }
}