using Flvt.Domain.Primitives.Responses;

namespace Flvt.Application.Subscribers.ResendEmail;

internal sealed class ResendEmailErrors
{
    public static Error InvalidPurpose => new ("Invalid purpose");
    public static Error PurposeNotSupported => new ("Purpose not supported");
}