using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.Primitives.Subscribers.Filters;
using Flvt.Domain.Subscribers;

namespace Flvt.Application.Advertisements.ProcessAdvertisements;

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
        var filter = Filter.Create(
            request.FilterName,
            request.Location,
            request.MinPrice,
            request.MaxPrice,
            request.MinRooms,
            request.MaxRooms,
            request.MinArea,
            request.MaxArea,
            null);

        if (filter.IsFailure)
        {
            return filter.Error;
        }

        var scrapedAdvertisements = await _scrapingOrchestrator.ScrapeAsync(filter.Value);

        await _processingOrchestrator.ProcessAsync(scrapedAdvertisements);

        return Result.Success();
    }
}
