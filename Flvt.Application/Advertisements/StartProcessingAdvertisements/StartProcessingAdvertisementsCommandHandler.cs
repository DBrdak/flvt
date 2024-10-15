using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ScrapedAdvertisements;

namespace Flvt.Application.Advertisements.StartProcessingAdvertisements;

internal sealed class StartProcessingAdvertisementsCommandHandler : ICommandHandler<StartProcessingAdvertisementsCommand>
{
    private readonly IProcessingOrchestrator _processingOrchestrator;
    private readonly IScrapedAdvertisementRepository _scrapedAdvertisementRepository;

    public StartProcessingAdvertisementsCommandHandler(IProcessingOrchestrator processingOrchestrator, IScrapedAdvertisementRepository scrapedAdvertisementRepository)
    {
        _processingOrchestrator = processingOrchestrator;
        _scrapedAdvertisementRepository = scrapedAdvertisementRepository;
    }

    public async Task<Result> Handle(StartProcessingAdvertisementsCommand request, CancellationToken cancellationToken)
    {
        var scrapedAdvertisementsGetResult = await _scrapedAdvertisementRepository.GetUnprocessedAsync();

        if (scrapedAdvertisementsGetResult.IsFailure)
        {
            return scrapedAdvertisementsGetResult.Error;
        }

        var scrapedAdvertisements = scrapedAdvertisementsGetResult.Value;

        var processingAdvertisements = await _processingOrchestrator.StartProcessingAsync(scrapedAdvertisements);

        var addTasks = processingAdvertisements.Select(pair => _scrapedAdvertisementRepository.AddRangeAsync(pair.Value));

        return Result.Aggregate(await Task.WhenAll(addTasks));
    }
}
