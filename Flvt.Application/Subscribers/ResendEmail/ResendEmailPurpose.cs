using Flvt.Domain.Primitives.Responses;

namespace Flvt.Application.Subscribers.ResendEmail;

internal sealed record ResendEmailPurpose
{
    public string Value { get; init; }

    private ResendEmailPurpose(string value) => Value = value;

    public static readonly ResendEmailPurpose Verification = new(nameof(Verification).ToLower());

    public static Result<ResendEmailPurpose> FromString(string value) =>
        value switch
        {
            _ when value.ToLower() == Verification.Value => Verification,
            _ => ResendEmailErrors.InvalidPurpose
        };
}