using Flvt.Application.Messaging;
using Flvt.Application.Subscribers.Models;
using Flvt.Domain.Filters;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.Subscribers;

namespace Flvt.Application.Subscribers.RemoveFilter;

internal sealed class RemoveFilterCommandHandler : ICommandHandler<RemoveFilterCommand, SubscriberModel>
{
    private readonly IFilterRepository _filterRepository;
    private readonly ISubscriberRepository _subscriberRepository;

    public RemoveFilterCommandHandler(
        IFilterRepository filterRepository,
        ISubscriberRepository subscriberRepository)
    {
        _filterRepository = filterRepository;
        _subscriberRepository = subscriberRepository;
    }

    public async Task<Result<SubscriberModel>> Handle(RemoveFilterCommand request, CancellationToken cancellationToken)
    {
        var subscriberGetResult = await _subscriberRepository.GetByEmailAsync(request.SubscriberEmail);

        if (subscriberGetResult.IsFailure)
        {
            return subscriberGetResult.Error;
        }

        var subscriber = subscriberGetResult.Value;

        var filterIdGetResult = subscriber.RemoveFilter(request.FilterId);

        if (filterIdGetResult.IsFailure)
        {
            return filterIdGetResult.Error;
        }

        var filterId = filterIdGetResult.Value;

        var filterRemoveTask =  _filterRepository.RemoveAsync(filterId);
        var subscriberUpdateTask = _subscriberRepository.UpdateAsync(subscriber);

        await Task.WhenAll(filterRemoveTask, subscriberUpdateTask);

        var filterRemoveResult = await filterRemoveTask;
        var subscriberUpdateResult = await subscriberUpdateTask;

        if (Result.Aggregate([filterRemoveResult, subscriberUpdateResult]) is var result && result.IsFailure)
        {
            return result.Error;
        }

        var filtersGetResult = await _filterRepository.GetManyByIdAsync(subscriber.Filters);

        if (filtersGetResult.IsFailure)
        {
            return filtersGetResult.Error;
        }

        var filters = filtersGetResult.Value.ToList();

        return SubscriberModel.FromDomain(subscriber, null, filters.Select(FilterModel.FromDomainModel));
    }
}
