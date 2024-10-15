using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ScrapedAdvertisements;

namespace Flvt.Application.Advertisements.ScrapeAdvertisements;

internal sealed class ScrapeAdvertisementsCommandHandler : ICommandHandler<ScrapeAdvertisementsCommand>
{
    private readonly IScrapingOrchestrator _scrapingOrchestrator;
    private readonly IScrapedAdvertisementRepository _scrapedAdvertisementRepository;
    private readonly IQueuePublisher _queuePublisher;

    public ScrapeAdvertisementsCommandHandler(IScrapingOrchestrator scrapingOrchestrator, IScrapedAdvertisementRepository scrapedAdvertisementRepository, IQueuePublisher queuePublisher)
    {
        _scrapingOrchestrator = scrapingOrchestrator;
        _scrapedAdvertisementRepository = scrapedAdvertisementRepository;
        _queuePublisher = queuePublisher;
    }

    public async Task<Result> Handle(ScrapeAdvertisementsCommand request, CancellationToken cancellationToken)
    {
        var filters = GlobalFilterFactory.CreateFiltersForAllLocations();
        var scrapeTasks = filters.Select(_scrapingOrchestrator.ScrapeAsync).ToList();
        
        var scrapedAdvertisements = await Task.WhenAll(scrapeTasks);

        var addTasks = scrapedAdvertisements.Select(_scrapedAdvertisementRepository.AddRangeAsync);

        var addResults = await Task.WhenAll(addTasks);

        if (Result.Aggregate(addResults) is var result && result.IsFailure)
        {
            return result.Error;
        }

        await _queuePublisher.PublishNewAdvertisements(cancellationToken);

        return Result.Success();
    }
}
