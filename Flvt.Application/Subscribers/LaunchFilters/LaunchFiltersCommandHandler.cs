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
    private readonly IFileService _fileService;

    public LaunchFiltersCommandHandler(
        IFilterRepository filterRepository,
        IProcessedAdvertisementRepository processedAdvertisementRepository,
        IQueuePublisher queuePublisher,
        IAdvertisementPhotosRepository photosRepository,
        IFileService fileService)
    {
        _filterRepository = filterRepository;
        _processedAdvertisementRepository = processedAdvertisementRepository;
        _queuePublisher = queuePublisher;
        _photosRepository = photosRepository;
        _fileService = fileService;
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

        var advertisements = advertisementsGetResult.Value.ToList();

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

        var photos = photosGetResult.Value.ToList();

        var advertisementsToFile = ProcessedAdvertisementModel.FromFilter(filter, advertisements, photos);

        var advertisementsFileWriteResult = await _fileService.WriteAdvertisementsToFileAsync(filter, advertisementsToFile);

        if (advertisementsFileWriteResult.IsFailure)
        {
            Log.Error(
                "Failed to write advertisements to file for filter {FilterId}, error :{error}",
                filter.Id,
                advertisementsFileWriteResult.Error);
            return null;
        }

        var advertisementsFilePath = advertisementsFileWriteResult.Value;

        filter.NewAdvertisementsFound(advertisementLinks, advertisementsFilePath);

        return filter;
    }
}
