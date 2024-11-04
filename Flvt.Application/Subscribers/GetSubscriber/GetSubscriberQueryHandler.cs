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

    public GetSubscriberQueryHandler(ISubscriberRepository subscriberRepository, IFilterRepository filterRepository)
    {
        _subscriberRepository = subscriberRepository;
        _filterRepository = filterRepository;
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

        Log.Logger.Information("Filters IDs: {filtersIds}", subscriber.Filters);
        Log.Logger.Information("Filters: {filters}", filters);

        var filterModels = filters.Select(FilterModel.FromDomainModel);

        return Result.Create(SubscriberModel.FromDomain(subscriber, null, filterModels));
    }
}