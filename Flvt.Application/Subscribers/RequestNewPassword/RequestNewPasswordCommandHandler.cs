using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.Subscribers;

namespace Flvt.Application.Subscribers.RequestNewPassword;

internal sealed class RequestNewPasswordCommandHandler : ICommandHandler<RequestNewPasswordCommand>
{
    private readonly ISubscriberRepository _subscriberRepository;
    private readonly IEmailService _emailService;

    public RequestNewPasswordCommandHandler(
        ISubscriberRepository subscriberRepository,
        IEmailService emailService)
    {
        _subscriberRepository = subscriberRepository;
        _emailService = emailService;
    }

    public async Task<Result> Handle(RequestNewPasswordCommand request, CancellationToken cancellationToken)
    {
        var subscriberGetResult = await _subscriberRepository.GetByEmailAsync(request.SubscriberEmail);

        if (subscriberGetResult.IsFailure)
        {
            return subscriberGetResult.Error;
        }

        var subscriber = subscriberGetResult.Value;

        var verificationCodeGenerateResult = subscriber.RequestNewPassword();

        if (verificationCodeGenerateResult.IsFailure)
        {
            return verificationCodeGenerateResult.Error;
        }

        await _emailService.SendResetPasswordEmailAsync(subscriber);

        return await _subscriberRepository.UpdateAsync(subscriber);
    }
}
