using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.Primitives.Subscribers.Filters;
using Flvt.Domain.ScrapedAdvertisements;

namespace Flvt.Application.Advertisements.ProcessAdvertisements;

internal sealed class ProcessAdvertisementsCommandHandler : ICommandHandler<ProcessAdvertisementsCommand>
{
    private readonly IScrapedAdvertisementRepository _scrapedAdvertisementRepository;
    private readonly IProcessingOrchestrator _processingOrchestrator;

    public ProcessAdvertisementsCommandHandler(IProcessingOrchestrator processingOrchestrator, IScrapedAdvertisementRepository scrapedAdvertisementRepository)
    {
        _processingOrchestrator = processingOrchestrator;
        _scrapedAdvertisementRepository = scrapedAdvertisementRepository;
    }

    public async Task<Result> Handle(ProcessAdvertisementsCommand request, CancellationToken cancellationToken)
    {
        var scrapedAdvertisementsGetResult = await _scrapedAdvertisementRepository.GetUnprocessedAsync();

        if (scrapedAdvertisementsGetResult.IsFailure)
        {
            return scrapedAdvertisementsGetResult.Error;
        }

        var scrapedAdvertisements = scrapedAdvertisementsGetResult.Value;

        await _processingOrchestrator.ProcessAsync(scrapedAdvertisements);

        return Result.Success();
    }
}
