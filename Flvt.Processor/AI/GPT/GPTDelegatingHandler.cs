namespace Flvt.Processor.AI.GPT;

internal sealed class GPTDelegatingHandler : DelegatingHandler
{
    private readonly GPTOptions _geminiOptions;

    public GPTDelegatingHandler(GPTOptions geminiOptions)
    {
        _geminiOptions = geminiOptions;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Add("Authorization", $"Bearer {_geminiOptions.ApiKey}");

        return base.SendAsync(request, cancellationToken);
    }
}