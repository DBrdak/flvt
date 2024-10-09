namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Runs.ValueObjects;

internal sealed record RequiredAction(
    string Type,
    string Description
);