using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Domain.Primitives;
using Flvt.Domain.Subscribers;

namespace Flvt.Application.ProcessAdvertisements;

internal sealed class ProcessAdvertisementsCommandHandler : ICommandHandler<ProcessAdvertisementsCommand>
{
    private readonly IScrapingOrchestrator _scrapingOrchestrator;
    private readonly IProcessingOrchestrator _processingOrchestrator;

    public ProcessAdvertisementsCommandHandler(
        IScrapingOrchestrator scrapingOrchestrator,
        IProcessingOrchestrator processingOrchestrator)
    {
        _scrapingOrchestrator = scrapingOrchestrator;
        _processingOrchestrator = processingOrchestrator;
    }

    public async Task<Result> Handle(ProcessAdvertisementsCommand request, CancellationToken cancellationToken)
    {
        var filter = new Filter()
            .InLocation(request.Location)
            .FromPrice(request.MinPrice)
            .ToPrice(request.MaxPrice)
            .FromRooms(request.MinRooms)
            .ToRooms(request.MaxRooms)
            .FromArea(request.MinArea)
            .ToArea(request.MaxArea)
            .Build();

        var scrapedAdvertisements = await _scrapingOrchestrator.ScrapeAsync(filter);

        await _processingOrchestrator.ProcessAsync(scrapedAdvertisements);

        return Result.Success();
    }
}
