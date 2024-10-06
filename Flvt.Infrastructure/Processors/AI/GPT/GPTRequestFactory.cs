using Flvt.Infrastructure.Processors.AI.GPT.Models.Request;

namespace Flvt.Infrastructure.Processors.AI.GPT;

internal sealed class GPTRequestFactory
{
    private const string userRole = "user";
    private const string gptModel = "gpt-4o-mini";

    public static GPTRequest CreateRequest(string message)
    {
        return new GPTRequest(gptModel, [new(userRole, message)]);
    }
}