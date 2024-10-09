namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Shared;

internal sealed record ToolResources(
    List<string>? FileIds,
    List<string>? VectorStoreIds);