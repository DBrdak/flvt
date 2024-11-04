using Flvt.Domain.Primitives.Responses;

namespace Flvt.Application.Subscribers.Register;

internal sealed class RegisterErrors
{
    public static readonly Error SubscriberAlreadyExists = new ("Subscriber with this email already exists");
}