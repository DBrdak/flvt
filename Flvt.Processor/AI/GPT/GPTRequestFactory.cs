using Flvt.Processor.AI.GPT.Models.Request;

namespace Flvt.Processor.AI.GPT;

internal sealed class GPTRequestFactory
{
    private const string userRole = "user";
    private const string gptModel = "gpt-4o";

    public static GPTRequest CreateRequest(string message)
    {
        return new GPTRequest(gptModel, [new(userRole, message)]);
    }
}