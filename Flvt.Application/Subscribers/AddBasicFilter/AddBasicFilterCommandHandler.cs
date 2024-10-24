using Flvt.Application.Messaging;
using Flvt.Application.Subscribers.Models;
using Flvt.Domain.Filters;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.Subscribers;

namespace Flvt.Application.Subscribers.AddBasicFilter;

internal sealed class AddBasicFilterCommandHandler : ICommandHandler<AddBasicFilterCommand, FilterModel>
{
    private readonly ISubscriberRepository _subscriberRepository;
    private readonly IFilterRepository _filterRepository;

    public AddBasicFilterCommandHandler(ISubscriberRepository subscriberRepository, IFilterRepository filterRepository)
    {
        _subscriberRepository = subscriberRepository;
        _filterRepository = filterRepository;
    }

    public async Task<Result<FilterModel>> Handle(AddBasicFilterCommand request, CancellationToken cancellationToken)
    {
        var subscriberGetResult = await _subscriberRepository.GetByEmailAsync(request.SubscriberEmail);

        if (subscriberGetResult.IsFailure)
        {
            return subscriberGetResult.Error;
        }

        var subscriber = subscriberGetResult.Value;

        var basicFilterCreateResult = subscriber.AddBasicFilter(
            request.Name,
            request.City,
            request.MinPrice,
            request.MaxPrice,
            request.MinRooms,
            request.MaxRooms,
            request.MinArea,
            request.MaxArea);

        if (basicFilterCreateResult.IsFailure)
        {
            return basicFilterCreateResult.Error;
        }

        var filter = basicFilterCreateResult.Value;

        var filterAddTask = _filterRepository.AddAsync(filter);
        var subscriberUpdateTask = _subscriberRepository.UpdateAsync(subscriber);

        await Task.WhenAll(filterAddTask, subscriberUpdateTask);

        if (Result.Aggregate([filterAddTask.Result, subscriberUpdateTask.Result]) is var result &&
            result.IsFailure)
        {
            return result.Error;
        }

        return FilterModel.FromDomainModel(filterAddTask.Result.Value);
    }
}
