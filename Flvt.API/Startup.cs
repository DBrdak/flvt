using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using Flvt.Application;
using Flvt.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flvt.API;

[Amazon.Lambda.Annotations.LambdaStartup]
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .AddSystemsManager("/flvt")
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
        services.AddAWSService<IAmazonS3>();

        services.AddApplication();
        services.AddInfrastructure();
    }
}