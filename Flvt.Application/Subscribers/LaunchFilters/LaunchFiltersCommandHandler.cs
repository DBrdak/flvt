using Flvt.Application.Abstractions;
using Flvt.Application.Advertisements.Models;
using Flvt.Application.Messaging;
using Flvt.Domain.Filters;
using Flvt.Domain.Photos;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ProcessedAdvertisements;
using Serilog;

namespace Flvt.Application.Subscribers.LaunchFilters;

internal sealed class LaunchFiltersCommandHandler : ICommandHandler<LaunchFiltersCommand>
{
    private readonly IFilterRepository _filterRepository;
    private readonly IProcessedAdvertisementRepository _processedAdvertisementRepository;
    private readonly IAdvertisementPhotosRepository _photosRepository;
    private readonly IQueuePublisher _queuePublisher;

    public LaunchFiltersCommandHandler(
        IFilterRepository filterRepository,
        IProcessedAdvertisementRepository processedAdvertisementRepository,
        IQueuePublisher queuePublisher,
        IAdvertisementPhotosRepository photosRepository)
    {
        _filterRepository = filterRepository;
        _processedAdvertisementRepository = processedAdvertisementRepository;
        _queuePublisher = queuePublisher;
        _photosRepository = photosRepository;
    }

    public async Task<Result> Handle(LaunchFiltersCommand request, CancellationToken cancellationToken)
    {
        var filterGetResult = await _filterRepository.GetAllAsync();

        if (filterGetResult.IsFailure)
        {
            return filterGetResult.Error;
        }

        var filters = filterGetResult.Value;

        var filtersToRun = GetFiltersToLaunch(filters);

        var filteringTasks = filtersToRun.Select(LaunchFilter);

        var launchedFiltersResults = await Task.WhenAll(filteringTasks);

        var launchedFilters = launchedFiltersResults.Where(filter => filter is not null)
            .Select(filter => filter!)
            .ToList();

        _filterRepository.StartBatchWrite();
        launchedFilters.ForEach(_filterRepository.AddItemToBatchWrite);
        
        var writeTask = _filterRepository.ExecuteBatchWriteAsync();

        var publishTask = _queuePublisher.PublishLaunchedFilters(launchedFilters);

        return Result.Aggregate(await Task.WhenAll(writeTask, publishTask));
    }

    private IEnumerable<Filter> GetFiltersToLaunch(IEnumerable<Filter> filters) => 
        filters.Where(filter => filter.ShouldLaunch);

    private async Task<Filter?> LaunchFilter(Filter filter)
    {
        var advertisementsGetResult = await _processedAdvertisementRepository.GetByFilterAsync(filter);

        if(advertisementsGetResult.IsFailure)
        {
            Log.Error(
                "Failed to get advertisements for filter {FilterId}, error :{error}",
                filter.Id,
                advertisementsGetResult.Error);
            return null;
        }

        var advertisements = advertisementsGetResult.Value;

        var advertisementLinks = advertisements
            .Select(advertisement => advertisement.Link)
            .ToList();

        var photosGetResult = await _photosRepository.GetByManyAdvertisementLinkAsync(advertisementLinks);

        if (photosGetResult.IsFailure)
        {
            Log.Error(
                "Failed to get advertisements photos for filter {FilterId}, error :{error}",
                filter.Id,
                photosGetResult.Error);
            return null;
        }

        var photos = photosGetResult.Value;

        var advertisementsToFile = ProcessedAdvertisementModel.FromDomainModel;

        filter.NewAdvertisementsFound(advertisementLinks, "");

        return filter;
    }
}
