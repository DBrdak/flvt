using Microsoft.Extensions.Options;

namespace Flvt.Infrastructure.Processors.AI.GPT.Options;

internal sealed class GPTDelegatingHandler : DelegatingHandler
{
    private readonly GPTOptions _gptOptions;

    public GPTDelegatingHandler(IOptions<GPTOptions> gptOptions)
    {
        _gptOptions = gptOptions.Value;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Add("Authorization", $"Bearer {_gptOptions.ApiKey}");
        request.Headers.Add("OpenAI-Beta", $"assistants=v2");

        return base.SendAsync(request, cancellationToken);
    }
}