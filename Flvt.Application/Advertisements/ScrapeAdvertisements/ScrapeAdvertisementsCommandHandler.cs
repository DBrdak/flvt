using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ScrapedAdvertisements;

namespace Flvt.Application.Advertisements.ScrapeAdvertisements;

//TODO Scrape advertisements for all available locations
internal sealed class ScrapeAdvertisementsCommandHandler : ICommandHandler<ScrapeAdvertisementsCommand>
{
    private readonly IScrapingOrchestrator _scrapingOrchestrator;
    private readonly IScrapedAdvertisementsRepository _scrapedAdvertisementsRepository;

    public ScrapeAdvertisementsCommandHandler(IScrapingOrchestrator scrapingOrchestrator, IScrapedAdvertisementsRepository scrapedAdvertisementsRepository)
    {
        _scrapingOrchestrator = scrapingOrchestrator;
        _scrapedAdvertisementsRepository = scrapedAdvertisementsRepository;
    }

    public async Task<Result> Handle(ScrapeAdvertisementsCommand request, CancellationToken cancellationToken)
    {
        var filters = GlobalFilterFactory.CreateFiltersForAllLocations();
        var scrapeTasks = filters.Select(_scrapingOrchestrator.ScrapeAsync).ToList();
        
        var scrapedAdvertisements = await Task.WhenAll(scrapeTasks);

        var addTasks = scrapedAdvertisements.Select(_scrapedAdvertisementsRepository.AddRangeAsync);

        var addResults = await Task.WhenAll(addTasks);

        if (Result.Aggregate(addResults) is var result && result.IsFailure)
        {
            return result.Error;
        }

        return Result.Success();
    }
}
