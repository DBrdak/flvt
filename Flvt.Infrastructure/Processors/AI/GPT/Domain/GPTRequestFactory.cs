namespace Flvt.Infrastructure.Processors.AI.GPT.Domain;

internal sealed class GPTRequestFactory
{
    private const string userRole = "user";

    public static MessageBody CreateMessage(string message) => new(userRole, message);
}