using Flvt.Domain.Subscribers;

namespace Flvt.Application.Subscribers.Models;

public sealed record SubscriberModel
{
    public string Email { get; init; }
    public bool IsEmailVerified { get; init; }
    public string Tier { get; init; }
    public string? Token { get; init; }
    public IReadOnlyCollection<FilterModel> Filters { get; init; }

    private SubscriberModel(
        string Email,
        string Tier,
        string? token,
        IReadOnlyCollection<FilterModel> filters,
        bool isEmailVerified)
    {
        this.Email = Email;
        this.Tier = Tier;
        Token = token;
        Filters = filters;
        IsEmailVerified = isEmailVerified;
    }

    internal static SubscriberModel FromDomain(Subscriber subscriber, string? token, IEnumerable<FilterModel> filters)
    {
        return new SubscriberModel(
            subscriber.Email.Value,
            subscriber.Tier.Value,
            token,
            filters.ToList(),
            subscriber.IsEmailVerified);
    }
}