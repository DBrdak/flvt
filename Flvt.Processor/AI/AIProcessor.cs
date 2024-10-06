using Flvt.Domain.Primitives;

namespace Flvt.Processor.AI;

internal sealed class AIProcessor
{

    private readonly GeminiClient _geminiClient;

    public LLMService(GeminiClient geminiClient)
    {
        _geminiClient = geminiClient;
    }

    public async Task<Result<string>> SendPromptAsync(Prompt prompt, CancellationToken cancellationToken)
    {
        var response = await _geminiClient.GenerateContentAsync(prompt.Value, cancellationToken);

        return response;
    }
}