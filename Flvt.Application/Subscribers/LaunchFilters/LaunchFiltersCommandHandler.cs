using Flvt.Application.Abstractions;
using Flvt.Application.Advertisements.Models;
using Flvt.Application.Messaging;
using Flvt.Application.Subscribers.Services;
using Flvt.Domain.Filters;
using Flvt.Domain.Photos;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ProcessedAdvertisements;
using Serilog;

namespace Flvt.Application.Subscribers.LaunchFilters;

internal sealed class LaunchFiltersCommandHandler : ICommandHandler<LaunchFiltersCommand>
{
    private readonly IFilterRepository _filterRepository;
    private readonly IQueuePublisher _queuePublisher;
    private readonly FiltersService _filtersService;

    public LaunchFiltersCommandHandler(
        IFilterRepository filterRepository,
        IQueuePublisher queuePublisher,
        FiltersService filtersService)
    {
        _filterRepository = filterRepository;
        _queuePublisher = queuePublisher;
        _filtersService = filtersService;
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

        var filteringTasks = filtersToRun.Select(_filtersService.LaunchFilter);

        var launchedFiltersResults = await Task.WhenAll(filteringTasks);

        var launchedFilters = launchedFiltersResults
            .Where(filter => filter is not null)
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
}
