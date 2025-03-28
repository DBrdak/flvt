using Amazon.S3;
using Flvt.Application;
using Flvt.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Events;
using Serilog;

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

        // AddLogger(services);
        // services.AddLogging(a => a.AddSerilog(null, true));
        services.AddApplication();
        services.AddInfrastructure();
    }
/*
    private static IServiceCollection AddLogger(IServiceCollection services)
    {
        var config = services
            .BuildServiceProvider()
            .GetRequiredService<IConfiguration>();

        Log.Logger = config["seq:uri"] switch
        {
            not null => new LoggerConfiguration()
                .WriteTo.Seq(
                    config["seq:uri"],
                    apiKey: config["seq:key"],
                    restrictedToMinimumLevel: LogEventLevel.Debug)
                .Enrich.FromLogContext()
                .CreateLogger(),
            _ => new LoggerConfiguration()
                .WriteTo.Console()
                .Enrich.FromLogContext()
                .CreateLogger()
        };

        return services;
    }
*/
}
