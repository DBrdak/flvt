namespace Flvt.Infrastructure.Processors.AI.GPT.Utils;

internal sealed class GPTPaths
{
    public static string RunThread => "v1/threads/runs";
    public static string RetrieveRun(string threadId, string runId) => $"v1/threads/{threadId}/runs/{runId}";

    public static string ListThreadMessages(string threadId, string runId, string after, int limit) =>
        $"v1/threads/{threadId}/messages?runId={runId}&after={after}&limit={limit}";
    public static string Assistants => "v1/assistants";
}