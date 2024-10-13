namespace Flvt.Infrastructure.Processors.AI.GPT.Utils;

internal sealed class GPTPaths
{
    public const string CreateCompletion = "v1/chat/completions";
    public const string UploadFile = "v1/files";
    public const string CreateBatch = "v1/batches";
    public static string RetrieveBatch(string batchId) => $"v1/batches/{batchId}";
}