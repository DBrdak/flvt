using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Application.Subscribers.Models;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.Subscribers;

namespace Flvt.Application.Subscribers.VerifyEmail;

internal sealed class VerifyEmailCommandHandler : ICommandHandler<VerifyEmailCommand, SubscriberModel>
{
    private readonly ISubscriberRepository _subscriberRepository;
    private readonly IJwtService _jwtService;

    public VerifyEmailCommandHandler(ISubscriberRepository subscriberRepository, IJwtService jwtService)
    {
        _subscriberRepository = subscriberRepository;
        _jwtService = jwtService;
    }

    public async Task<Result<SubscriberModel>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var subscriberGetResult = await _subscriberRepository.GetByEmailAsync(request.Email);

        if (subscriberGetResult.IsFailure)
        {
            return subscriberGetResult.Error;
        }

        var subscriber = subscriberGetResult.Value;

        var verificationResult = subscriber.VerifyEmail(request.VerificationCode);

        if (verificationResult.IsFailure)
        {
            await _subscriberRepository.UpdateAsync(subscriber);
            return verificationResult.Error;
        }

        var tokenGenerateResult = _jwtService.GenerateJwt(subscriber);

        if (tokenGenerateResult.IsFailure)
        {
            return tokenGenerateResult.Error;
        }

        var token = tokenGenerateResult.Value;

        var updateResult = await _subscriberRepository.UpdateAsync(subscriber);

        if (updateResult.IsFailure)
        {
            return updateResult.Error;
        }

        return SubscriberModel.FromDomain(subscriber, token, []);
    }
}
