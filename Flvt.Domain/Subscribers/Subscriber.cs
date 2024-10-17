using Flvt.Domain.Primitives.Advertisements;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.Primitives.Subscribers;
using Flvt.Domain.Primitives.Subscribers.Filters;

namespace Flvt.Domain.Subscribers;

public sealed class Subscriber
{
    public Email Email { get; init; }
    public SubscribtionTier Tier { get; init; }
    public List<Filter> Filters { get; init; }
    public Country Country { get; init; }

    private Subscriber(Email email, List<Filter> filters, Country country)
    {
        Email = email;
        Filters = filters;
        Country = country;
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

        return new Subscriber(emailResult.Value, new List<Filter>(), countryResult.Value);
    }

    public void AddBasicFilter()
    {

    }
}

public sealed class SubscribtionTier
{
    public string Value { get; init; }

    public SubscribtionTier(string value) => Value = value;

    public static SubscribtionTier Basic => new ("Basic");
    public static SubscribtionTier Silver => new ("Silver");
    public static SubscribtionTier Gold => new ("Gold");
    public static SubscribtionTier Platinum => new ("Platinum");
    public static SubscribtionTier Ruby => new ("Ruby");
    public static SubscribtionTier Sapphire => new ("Sapphire");
    public static SubscribtionTier Diamond => new ("Diamond");

    private static readonly IReadOnlyCollection<SubscribtionTier> all =
    [
        Basic, Silver, Gold, Platinum, Ruby, Sapphire, Diamond
    ];

    public static Result<SubscribtionTier> Create(string value) =>
        Result.Create(
            all.FirstOrDefault(tier => tier.Value == value),
            new Error("Invalid subscribtion tier."));
}