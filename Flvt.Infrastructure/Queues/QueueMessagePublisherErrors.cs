using Flvt.Domain.Primitives.Responses;

namespace Flvt.Infrastructure.Queues;

internal sealed class QueueMessagePublisherErrors
{
    public static readonly Error ConnectionError = new(
        "Error occured when trying to establish the connection with SQS service");

    public static readonly Error UnknownError = new(
        "Unknown error has occured when trying to communicate with SQS service");
}