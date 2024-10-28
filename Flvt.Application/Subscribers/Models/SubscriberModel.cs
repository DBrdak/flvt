using Flvt.Domain.Subscribers;

namespace Flvt.Application.Subscribers.Models;

public sealed record SubscriberModel
{
    public string Email { get; init; }
    public string Tier { get; init; }
    public string Country { get; init; }
    public string? Token { get; init; }
    public IReadOnlyCollection<FilterModel> Filters { get; init; }

    private SubscriberModel(
        string Email,
        string Tier,
        string Country,
        string? token,
        IReadOnlyCollection<FilterModel> filters)
    {
        this.Email = Email;
        this.Tier = Tier;
        this.Country = Country;
        Token = token;
        Filters = filters;
    }

    internal static SubscriberModel FromDomain(Subscriber subscriber, string? token, IEnumerable<FilterModel> filters)
    {
        return new SubscriberModel(
            subscriber.Email.Value,
            subscriber.Tier.Value,
            subscriber.Country.Code,
            token,
            filters.ToList());
    }
}