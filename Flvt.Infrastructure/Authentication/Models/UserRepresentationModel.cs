using System.Security.Claims;
using Flvt.Domain.Subscribers;

namespace Flvt.Infrastructure.Authentication.Models;

public sealed record UserRepresentationModel
{
    public static readonly string TokenCratedTimeStampClaimName = nameof(TokenCreatedTimestamp);
    public long TokenCreatedTimestamp { get; init; }

    public static readonly string EmailClaimName = nameof(Email);
    public string Email { get; init; }

    public static readonly string CountryClaimName = nameof(Country);
    public string Country { get; init; }

    public static readonly string EmailVerifiedClaimName = nameof(EmailVerified);
    public bool EmailVerified { get; init; }

    private UserRepresentationModel(
        long tokenCreatedTimestamp,
        string email,
        bool emailVerified,
        string country)
    {
        TokenCreatedTimestamp = tokenCreatedTimestamp;
        Email = email;
        EmailVerified = emailVerified;
        Country = country;
    }

    internal static UserRepresentationModel FromUser(Subscriber user) =>
        new(
            DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            user.Email.Value,
            user.IsEmailVerified,
            user.Country.Code);

    internal Claim[] ToClaims()
    {
        return
        [
            new Claim(TokenCratedTimeStampClaimName, TokenCreatedTimestamp.ToString()),
            new Claim(EmailClaimName, Email),
            new Claim(EmailVerifiedClaimName, EmailVerified.ToString()),
            new Claim(CountryClaimName, Country)
        ];
    }
}