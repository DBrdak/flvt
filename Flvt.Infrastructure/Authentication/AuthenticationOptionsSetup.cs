using Flvt.Infrastructure.SecretManager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Flvt.Infrastructure.Authentication;

public sealed class AuthenticationOptionsSetup : IConfigureOptions<AuthenticationOptions>
{
    private const string sectionName = "authentication";
    private readonly IConfiguration _configuration;

    public AuthenticationOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(AuthenticationOptions options)
    {
        _configuration.GetSection(sectionName).Bind(options);
    }
}