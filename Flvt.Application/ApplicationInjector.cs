using Amazon.CloudWatchLogs;
using Flvt.Application.Behaviors;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Sinks.AwsCloudWatch;

namespace Flvt.Application;

public static class ApplicationInjector
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(
            config =>
            {
                config.RegisterServicesFromAssemblyContaining<ApplicationAssemblyReference>();
                config.AddOpenBehavior(typeof(TrackingBehavior<,>));
            });

        services.AddLogger();

        return services;
    }

    private static IServiceCollection AddLogger(this IServiceCollection services)
    {
        var options = new CloudWatchSinkOptions
        {
            LogGroupName = "flvt",
            LogStreamNameProvider = new DefaultLogStreamProvider(),
            TextFormatter = new JsonFormatter()
        };

        var client = new AmazonCloudWatchLogsClient();
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.AmazonCloudWatch(options, client)
            .WriteTo.Console()
            .CreateLogger();

        return services;
    }
}