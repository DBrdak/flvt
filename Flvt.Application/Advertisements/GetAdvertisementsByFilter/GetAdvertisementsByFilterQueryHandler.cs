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

        //TODO Implement pagination
        //What if I will save found advertisements of filter somewhere (dynamodb or s3) and then return them by page?
        // TODO What should I have to do before frontend?
        // 1. Implement ads get
        // 2. Implement authentication
        // 3. Expose API
        // 4. Configure background jobs

        return null;
    }
}