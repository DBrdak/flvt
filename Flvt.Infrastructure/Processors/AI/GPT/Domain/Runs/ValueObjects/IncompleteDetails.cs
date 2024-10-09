namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Runs.ValueObjects;

internal sealed record IncompleteDetails(
    string Reason,
    string Details
);