using Flvt.Domain.Primitives.Responses;

namespace Flvt.Domain.Subscribers;

internal sealed class SubscriberErrors
{
    public static Error InvalidFilterTier(string expectedTier, string actualTier) =>
        new($"Invalid filter tier, expected: {expectedTier}, actual: {actualTier}");
    public static Error ActionNotAllowedForBasicSubscribers =>
        new ("This action is not allowed for Basic tier subscribers");
    public static Error FilterNotFound => new("Filter not found");
    internal static Error UserLocked(int seconds) => new(
        $"User is locked for {seconds} seconds");
    public static Error PasswordTooWeak => new("Password is too weak");
    public static Error InvalidCredentials => new("Invalid credentials");
    public static Error EmailNotVerified => new("Email is not verified");
    public static Error VerificationCodeCannotBeReGenerated => new("Verification code cannot be re-generated");
    public static Error VerificationCodeInvalid => new("Verification code is invalid");
}