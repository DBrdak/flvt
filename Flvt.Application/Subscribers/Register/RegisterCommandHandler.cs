using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.Subscribers;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Flvt.Application.Subscribers.Register;

internal sealed class RegisterCommandHandler : ICommandHandler<RegisterCommand>
{
    private readonly ISubscriberRepository _subscriberRepository;
    private readonly IEmailService _emailService;

    public RegisterCommandHandler(ISubscriberRepository subscriberRepository, IEmailService emailService)
    {
        _subscriberRepository = subscriberRepository;
        _emailService = emailService;
    }

    public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var registerResult = Subscriber.Register(request.Email, request.CountryCode, request.Password);

        if (registerResult.IsFailure)
        {
            return registerResult.Error;
        }

        var subscriber = registerResult.Value;

        var addResult = await _subscriberRepository.AddAsync(subscriber);

        if (addResult.IsFailure)
        {
            return addResult.Error;
        }

        var sendEmailResult = await _emailService.SendVerificationEmailAsync(subscriber);

        if (sendEmailResult.IsFailure)
        {
            return sendEmailResult.Error;
        }

        return Result.Success();
    }
}
