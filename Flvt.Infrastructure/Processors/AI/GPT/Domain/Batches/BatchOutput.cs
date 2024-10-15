using Flvt.Infrastructure.Processors.AI.GPT.Domain.Chat.Completions;
using Flvt.Infrastructure.Processors.AI.GPT.Utils;

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

internal sealed record ChatCompletionBatchResponse(
    int StatusCode,
    string RequestId,
    ChatCompletion Body
)
{
    public static ChatCompletionBatchResponse? FromBatchResponse(BatchResponse? batchResponse)
    {
        var jsonResponse = GPTJsonConvert.Serialize(batchResponse);
        var completion = GPTJsonConvert.DeserializeObject<ChatCompletionBatchResponse>(jsonResponse);

        return completion;
    }
}