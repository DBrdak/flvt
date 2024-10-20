using Flvt.Domain.Primitives.Responses;

namespace Flvt.Domain.Subscribers;

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