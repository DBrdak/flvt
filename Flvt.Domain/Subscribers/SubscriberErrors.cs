using Flvt.Domain.Primitives.Responses;

namespace Flvt.Domain.Subscribers;

internal sealed class SubscriberErrors
{
    public static Error InvalidFilterTier(string expectedTier, string actualTier) =>
        new($"Invalid filter tier, expected: {expectedTier}, actual: {actualTier}");
    public static Error ActionNotAllowedForBasicSubscribers =>
        new ("This action is not allowed for Basic tier subscribers");
}