using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ScrapedAdvertisements;

namespace Flvt.Application.Advertisements.ScrapeAdvertisements;

internal sealed class ScrapeAdvertisementsCommandHandler : ICommandHandler<ScrapeAdvertisementsCommand>
{
    private readonly IScrapingOrchestrator _scrapingOrchestrator;
    private readonly IScrapedAdvertisementRepository _scrapedAdvertisementRepository;

    public ScrapeAdvertisementsCommandHandler(
        IScrapingOrchestrator scrapingOrchestrator,
        IScrapedAdvertisementRepository scrapedAdvertisementRepository)
    {
        _scrapingOrchestrator = scrapingOrchestrator;
        _scrapedAdvertisementRepository = scrapedAdvertisementRepository;
    }

    public async Task<Result> Handle(ScrapeAdvertisementsCommand request, CancellationToken cancellationToken)
    {
        var filters = GlobalFilterFactory.CreateFiltersForAllLocations();
        var scrapeTasks = filters.Select(_scrapingOrchestrator.ScrapeAsync).ToList();
        
        var scrapedAdvertisements = await Task.WhenAll(scrapeTasks);

        _scrapedAdvertisementRepository.StartBatchWrite();

        scrapedAdvertisements
            .ToList()
            .ForEach(_scrapedAdvertisementRepository.AddManyItemsToBatchWrite);

        return await _scrapedAdvertisementRepository.ExecuteBatchWriteAsync();
    }
}
