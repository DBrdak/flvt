using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Application.Subscribers.Models;
using Flvt.Domain.Filters;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.Subscribers;
using Serilog;

namespace Flvt.Application.Subscribers.GetSubscriber;

internal sealed class GetSubscriberQueryHandler : IQueryHandler<GetSubscriberQuery, SubscriberModel>
{
    private readonly ISubscriberRepository _subscriberRepository;
    private readonly IFilterRepository _filterRepository;
    private readonly IJwtService _jwtService;

    public GetSubscriberQueryHandler(ISubscriberRepository subscriberRepository, IFilterRepository filterRepository, IJwtService jwtService)
    {
        _subscriberRepository = subscriberRepository;
        _filterRepository = filterRepository;
        _jwtService = jwtService;
    }

    public async Task<Result<SubscriberModel>> Handle(GetSubscriberQuery request, CancellationToken cancellationToken)
    {
        var subscriberGetResult = await _subscriberRepository.GetByEmailAsync(request.SubscriberEmail);

        if (subscriberGetResult.IsFailure)
        {
            return subscriberGetResult.Error;
        }

        var subscriber = subscriberGetResult.Value;

        var filtersGetResult = await _filterRepository.GetManyByIdAsync(subscriber.Filters);

        if (filtersGetResult.IsFailure)
        {
            return filtersGetResult.Error;
        }

        var filters = filtersGetResult.Value.ToList();

        var filterModels = filters.Select(FilterModel.FromDomainModel);

        return Result.Create(SubscriberModel.FromDomain(subscriber, null, filterModels));
    }
}