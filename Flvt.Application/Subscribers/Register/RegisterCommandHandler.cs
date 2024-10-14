using Flvt.Application.Messaging;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.Subscribers;

namespace Flvt.Application.Subscribers.Register;

internal sealed class RegisterCommandHandler : ICommandHandler<RegisterCommand>
{
    private readonly ISubscriberRepository _subscriberRepository;

    public RegisterCommandHandler(ISubscriberRepository subscriberRepository)
    {
        _subscriberRepository = subscriberRepository;
    }

    public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var registerResult = Subscriber.Register(request.Email, request.CountryCode);

        if (registerResult.IsFailure)
        {
            return registerResult.Error;
        }

        var subscriber = registerResult.Value;

        return await _subscriberRepository.AddAsync(subscriber);
    }
}
