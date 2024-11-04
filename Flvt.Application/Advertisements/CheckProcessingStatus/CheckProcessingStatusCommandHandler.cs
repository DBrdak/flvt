using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Domain.Primitives.Responses;

namespace Flvt.Application.Advertisements.CheckProcessingStatus;

internal sealed class CheckProcessingStatusCommandHandler : ICommandHandler<CheckProcessingStatusCommand>
{
    private readonly IProcessingOrchestrator _processingOrchestrator;
    private readonly IQueuePublisher _queuePublisher;

    public CheckProcessingStatusCommandHandler(
        IProcessingOrchestrator processingOrchestrator,
        IQueuePublisher queuePublisher)
    {
        _processingOrchestrator = processingOrchestrator;
        _queuePublisher = queuePublisher;
    }

    public async Task<Result> Handle(CheckProcessingStatusCommand request, CancellationToken cancellationToken)
    {
        var isAnyFinished = await _processingOrchestrator.CheckIfAnyBatchFinishedAsync();

        if (!isAnyFinished)
        {
            return Result.Success();
        }

        return await _queuePublisher.PublishFinishedBatches();
    }
}
