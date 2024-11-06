using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.Subscribers;
using Serilog;

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
        var subscriberEmailCreateResult = Email.Create(request.SubscriberEmail);

        if (subscriberEmailCreateResult.IsFailure)
        {
            return subscriberEmailCreateResult.Error;
        }

        var subscriberEmail = subscriberEmailCreateResult.Value;

        var subscriberGetResult = await _subscriberRepository.GetByEmailAsync(subscriberEmail.Value);

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

        var sendResult = await _emailService.SendResetPasswordEmailAsync(subscriber);

        if (sendResult.IsFailure)
        {
            return sendResult.Error;
        }

        var updateResult = await _subscriberRepository.UpdateAsync(subscriber);

        return updateResult.IsFailure ? updateResult.Error : Result.Success();
    }
}
