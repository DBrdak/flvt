using Flvt.Infrastructure.Processors.AI.GPT.Utils;

namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Batches.Create.Request;

internal sealed record BatchCreateRequest(
    string InputFileId,
    Dictionary<string, string> Metadata,
    string Endpoint = GPTPaths.CreateCompletion,
    string CompletionWindow = "24h")
{
}