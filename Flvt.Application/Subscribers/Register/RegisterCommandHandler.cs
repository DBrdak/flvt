using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Application.Subscribers.Models;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.Subscribers;

namespace Flvt.Application.Subscribers.Register;

internal sealed class RegisterCommandHandler : ICommandHandler<RegisterCommand, SubscriberModel>
{
    private readonly ISubscriberRepository _subscriberRepository;
    private readonly IEmailService _emailService;
    private readonly IJwtService _jwtService;

    public RegisterCommandHandler(
        ISubscriberRepository subscriberRepository,
        IEmailService emailService,
        IJwtService jwtService)
    {
        _subscriberRepository = subscriberRepository;
        _emailService = emailService;
        _jwtService = jwtService;
    }

    public async Task<Result<SubscriberModel>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var registerResult = Subscriber.Register(request.Email, request.Password);

        if (registerResult.IsFailure)
        {
            return registerResult.Error;
        }

        var subscriber = registerResult.Value;

        var existingSubscriberGetResult = await _subscriberRepository.GetByEmailAsync(subscriber.Email.Value);

        if (existingSubscriberGetResult.IsSuccess)
        {
            return RegisterErrors.SubscriberAlreadyExists;
        }

        var tokenCreateResult = _jwtService.GenerateJwt(subscriber);

        if (tokenCreateResult.IsFailure)
        {
            return tokenCreateResult.Error;
        }

        var token = tokenCreateResult.Value;

        var addResult = await _subscriberRepository.AddAsync(subscriber);

        if (addResult.IsFailure)
        {
            return addResult.Error;
        }

        var sendEmailResult = await _emailService.SendVerificationEmailAsync(subscriber);

        if (sendEmailResult.IsFailure)
        {
            await _subscriberRepository.RemoveAsync(subscriber.Email.Value);
            return sendEmailResult.Error;
        }

        return SubscriberModel.FromDomain(subscriber, token, []);
    }
}