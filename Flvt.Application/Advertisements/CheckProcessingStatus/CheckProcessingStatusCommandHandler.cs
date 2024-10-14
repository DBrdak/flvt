using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ProcessedAdvertisements;

namespace Flvt.Application.Advertisements.CheckProcessingStatus;

internal sealed class CheckProcessingStatusCommandHandler : ICommandHandler<CheckProcessingResultsCommand>
{
    private readonly IProcessingOrchestrator _processingOrchestrator;
    private readonly IProcessedAdvertisementRepository _processedAdvertisementRepository;

    public CheckProcessingStatusCommandHandler(
        IProcessingOrchestrator processingOrchestrator,
        IProcessedAdvertisementRepository processedAdvertisementRepository)
    {
        _processingOrchestrator = processingOrchestrator;
        _processedAdvertisementRepository = processedAdvertisementRepository;
    }

    public async Task<Result> Handle(CheckProcessingResultsCommand request, CancellationToken cancellationToken)
    {
        var processedAdvertisements = await _processingOrchestrator.RetrieveProcessedAdvertisementsAsync();

        return await _processedAdvertisementRepository.AddRangeAsync(processedAdvertisements);
    }
}
