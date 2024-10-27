using Flvt.Application.Advertisements.Models;
using Flvt.Application.Messaging;
using Flvt.Domain.Filters;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ProcessedAdvertisements;
using Flvt.Domain.Subscribers;

namespace Flvt.Application.Advertisements.GetAdvertisementsByFilter;

internal sealed class GetAdvertisementsByFilterQueryHandler : IQueryHandler<GetAdvertisementsByFilterQuery, Page<ProcessedAdvertisementModel>>
{
    private readonly ISubscriberRepository _subscriberRepository;
    private readonly IFilterRepository _filterRepository;
    private readonly IProcessedAdvertisementRepository _processedAdvertisementRepository;

    public GetAdvertisementsByFilterQueryHandler(
        ISubscriberRepository subscriberRepository,
        IFilterRepository filterRepository,
        IProcessedAdvertisementRepository processedAdvertisementRepository)
    {
        _subscriberRepository = subscriberRepository;
        _filterRepository = filterRepository;
        _processedAdvertisementRepository = processedAdvertisementRepository;
    }

    public async Task<Result<Page<ProcessedAdvertisementModel>>> Handle(
        GetAdvertisementsByFilterQuery request,
        CancellationToken cancellationToken)
    {
        if (request.SubscriberEmail is null ||
            request.FilterId is null ||
            request.Page is null ||
            request.PageSize is null)
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

        _processedAdvertisementRepository.StartBatchGet();
        filter.FoundAdvertisements.ToList().ForEach(_processedAdvertisementRepository.AddItemToBatchGet);
        var advertisementsGetResult = await _processedAdvertisementRepository.ExecuteBatchGetAsync();

        if (advertisementsGetResult.IsFailure)
        {
            return advertisementsGetResult.Error;
        }

        var advertisements = advertisementsGetResult.Value;

        

        return null;
    }
}

internal sealed class GetAdvertisementsByFilterErrors
{
    public static readonly Error InvalidRequest =
        new("Invalid request, one or more of required parameters missing: Page / FilterId / SubscriberEmail");
}
