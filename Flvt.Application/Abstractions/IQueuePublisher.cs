using Flvt.Domain.Primitives.Responses;

namespace Flvt.Application.Abstractions;

public interface IQueuePublisher
{
    Task<Result> PublishFinishedBatches(CancellationToken cancellationToken);
    Task<Result> PublishNewAdvertisements(CancellationToken cancellationToken);
}