using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Domain.Filters;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.Subscribers;

namespace Flvt.Application.Advertisements.GetAdvertisementsByFilter;

internal sealed class GetAdvertisementsByFilterQueryHandler : IQueryHandler<GetAdvertisementsByFilterQuery, string>
{
    private readonly ISubscriberRepository _subscriberRepository;
    private readonly IFilterRepository _filterRepository;
    private readonly IFileService _fileService;

    public GetAdvertisementsByFilterQueryHandler(
        ISubscriberRepository subscriberRepository,
        IFilterRepository filterRepository,
        IFileService fileService)
    {
        _subscriberRepository = subscriberRepository;
        _filterRepository = filterRepository;
        _fileService = fileService;
    }

    public async Task<Result<string>> Handle(
        GetAdvertisementsByFilterQuery request,
        CancellationToken cancellationToken)
    {
        if (request.SubscriberEmail is null ||
            request.FilterId is null)
        {
            return GetAdvertisementsByFilterErrors.InvalidRequest;
        }

        var subscriberGetResult = await _subscriberRepository.GetByEmailAsync(request.SubscriberEmail);

        if (subscriberGetResult.IsFailure)
        {
            return subscriberGetResult.Error;
        }

        var subscriber = subscriberGetResult.Value;

        var filterIdGetResult = subscriber.GetFilter(request.FilterId);

        if (filterIdGetResult.IsFailure)
        {
            return filterIdGetResult.Error;
        }

        var filterId = filterIdGetResult.Value;

        var filterGetResult = await _filterRepository.GetByIdAsync(filterId);

        if (filterGetResult.IsFailure)
        {
            return filterGetResult.Error;
        }

        var filter = filterGetResult.Value;

        return await _fileService.GetAdvertisementsUrlAsync(filter);

        // TODO What should I have to do before frontend?
        // 5. Email on launched filter
    }
}