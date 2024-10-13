using Flvt.Domain.Primitives.Advertisements;
using Flvt.Domain.Primitives.Subscribers;
using Flvt.Domain.Primitives.Subscribers.Filters;

namespace Flvt.Domain.Subscribers;

public sealed class Subscriber
{
    public Email Email { get; init; }
    public IEnumerable<Filter> Filter { get; init; }
    public Country Country { get; init; }

    public Subscriber(Email email, IEnumerable<Filter> filter, Country country)
    {
        Email = email;
        Filter = filter;
        Country = country;
    }
}