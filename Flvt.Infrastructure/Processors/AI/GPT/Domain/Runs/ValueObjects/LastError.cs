namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Runs.ValueObjects;

internal sealed record LastError(
    string Code,
    string Message
);