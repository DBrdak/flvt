namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Runs.ValueObjects;

internal sealed record RunStatus()
{
    public static readonly string Queued = "queued";
    public static readonly string InProgress = "in_progress";
    public static readonly string RequiresAction = "requires_action";
    public static readonly string Cancelling = "cancelling";
    public static readonly string Cancelled = "cancelled";
    public static readonly string Failed = "failed";
    public static readonly string Completed = "completed";
    public static readonly string Incomplete = "incomplete";
    public static readonly string Expired = "expired";

}