using Flvt.Domain.Primitives;
using Flvt.Infrastructure.Processors.AI.GPT;
using Flvt.Infrastructure.Processors.AI.Models;

namespace Flvt.Infrastructure.Processors.AI;

internal sealed class AIProcessor
{

    private readonly GPTClient _gptClient;

    public AIProcessor(GPTClient gptClient)
    {
        _gptClient = gptClient;
    }

    public async Task<Result<string>> SendPromptAsync(Prompt prompt, CancellationToken cancellationToken)
    {
        var response = await _gptClient.MessageAsync(prompt.ToString(), cancellationToken);

        return response;
    }
}