namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Batches;

internal sealed record BatchOutput(
    string Id,
    string CustomId,
    BatchResponse? Response,
    string? Error
);

internal sealed record BatchResponse(
    int StatusCode,
    string RequestId,
    object Body
);