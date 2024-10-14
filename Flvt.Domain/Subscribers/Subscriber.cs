using Flvt.Domain.Primitives.Advertisements;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.Primitives.Subscribers;
using Flvt.Domain.Primitives.Subscribers.Filters;

namespace Flvt.Domain.Subscribers;

public sealed class Subscriber
{
    public Email Email { get; init; }
    public List<Filter> Filter { get; init; }
    public Country Country { get; init; }

    private Subscriber(Email email, List<Filter> filter, Country country)
    {
        Email = email;
        Filter = filter;
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