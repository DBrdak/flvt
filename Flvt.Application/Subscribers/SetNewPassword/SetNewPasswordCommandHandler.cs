using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Application.Subscribers.Models;
using Flvt.Domain.Filters;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.Subscribers;

namespace Flvt.Application.Subscribers.SetNewPassword;

internal sealed class SetNewPasswordCommandHandler : ICommandHandler<SetNewPasswordCommand, SubscriberModel>
{
    private readonly ISubscriberRepository _subscriberRepository;
    private readonly IJwtService _jwtService;
    private readonly IFilterRepository _filterRepository;

    public SetNewPasswordCommandHandler(
        ISubscriberRepository subscriberRepository,
        IJwtService jwtService,
        IFilterRepository filterRepository)
    {
        _subscriberRepository = subscriberRepository;
        _jwtService = jwtService;
        _filterRepository = filterRepository;
    }

    public async Task<Result<SubscriberModel>> Handle(SetNewPasswordCommand request, CancellationToken cancellationToken)
    {
        var subscriberGetResult = await _subscriberRepository.GetByEmailAsync(request.SubscriberEmail);

        if (subscriberGetResult.IsFailure)
        {
            return subscriberGetResult.Error;
        }

        var subscriber = subscriberGetResult.Value;

        var changePasswordResult = subscriber.ChangePassword(request.NewPassword, request.VerificationCode);

        if (changePasswordResult.IsFailure)
        {
            return changePasswordResult.Error;
        }

        var updateResult = await _subscriberRepository.UpdateAsync(subscriber);

        if (updateResult.IsFailure)
        {
            return updateResult.Error;
        }

        var tokenGenerateResult = _jwtService.GenerateJwt(subscriber);

        if (tokenGenerateResult.IsFailure)
        {
            return tokenGenerateResult.Error;
        }

        var token = tokenGenerateResult.Value;

        var filtersGetResult = await _filterRepository.GetManyByIdAsync(subscriber.Filters);

        if (filtersGetResult.IsFailure)
        {
            return filtersGetResult.Error;
        }

        var filters = filtersGetResult.Value.ToList();

        return SubscriberModel.FromDomain(subscriber, token, filters.Select(FilterModel.FromDomainModel));
    }
}
