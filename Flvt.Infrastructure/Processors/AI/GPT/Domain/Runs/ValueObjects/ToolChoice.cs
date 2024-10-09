namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Runs.ValueObjects;

internal sealed record ToolChoice(
    string Type,
    Function? Function
);