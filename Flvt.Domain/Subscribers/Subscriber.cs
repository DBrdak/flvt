using Flvt.Domain.Primitives;

namespace Flvt.Domain.Subscribers;

public sealed class Subscriber
{
    public Email Email { get; init; }
    public Filter Filter { get; init; }
    public Preferences? Preferences { get; init; }
    public Country Country { get; init; }

    public Subscriber(Email email, Filter filter, Preferences preferences, Country country)
    {
        Email = email;
        Filter = filter;
        Country = country;
    }
}