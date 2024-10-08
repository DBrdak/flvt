using Flvt.Domain;
using Flvt.Infrastructure.SecretManager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Flvt.Infrastructure.Processors.AI.GPT.Options;

internal sealed class GPTOptionsSetup : IConfigureOptions<GPTOptions>
{
    private const string sectionName = "gpt_key";
    private readonly IConfiguration _configuration;
    private const string secretName = "flvt/gpt_key";
    private const string baseUrl = "https://api.openai.com";

    public GPTOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(GPTOptions options)
    {
        _configuration.GetSection(sectionName).Bind(options);

        options.ApiKey = XD.LOL; // TODO Remove

        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            options.ApiKey = SecretAccesor.GetSecret(secretName);
        }

        if (string.IsNullOrWhiteSpace(options.Url))
        {
            options.Url = baseUrl;
        }
    }
}