using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.Subscribers;

namespace Flvt.Application.Subscribers.ResendEmail;

internal sealed class ResendEmailCommandHandler : ICommandHandler<ResendEmailCommand>
{
    private readonly ISubscriberRepository _subscriberRepository;
    private readonly IEmailService _emailService;

    public ResendEmailCommandHandler(ISubscriberRepository subscriberRepository, IEmailService emailService)
    {
        _subscriberRepository = subscriberRepository;
        _emailService = emailService;
    }

    public async Task<Result> Handle(ResendEmailCommand request, CancellationToken cancellationToken)
    {
        var subscriberGetResult = await _subscriberRepository.GetByEmailAsync(request.Email);

        if (subscriberGetResult.IsFailure)
        {
            return subscriberGetResult.Error;
        }

        var subscriber = subscriberGetResult.Value;

        var purposeCreateResult = ResendEmailPurpose.FromString(request.Purpose);

        if (purposeCreateResult.IsFailure)
        {
            return purposeCreateResult.Error;
        }

        var purpose = purposeCreateResult.Value;

        subscriber.ReGenerateVerificationCode();

        var updateResult = await _subscriberRepository.UpdateAsync(subscriber);

        if (updateResult.IsFailure)
        {
            return updateResult.Error;
        }

        return purpose switch
        {
            _ when purpose == ResendEmailPurpose.Verification => 
                await _emailService.SendVerificationEmailAsync(subscriber),
            _ when purpose == ResendEmailPurpose.NewPassword => 
                await _emailService.SendResetPasswordEmailAsync(subscriber),
            _ => ResendEmailErrors.PurposeNotSupported
        };
    }
}
