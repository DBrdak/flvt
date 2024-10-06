using Amazon.CloudWatchLogs;
using Amazon.Runtime;
using Flvt.Application.Abstractions;
using Flvt.Infrastructure.Monitoring;
using Flvt.Infrastructure.Processors;
using Flvt.Infrastructure.Processors.AI.GPT;
using Flvt.Infrastructure.Scrapers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog.Sinks.AwsCloudWatch;

namespace Flvt.Infrastructure;

public static class InfrastructureInjector
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ILoggingService, LoggingService>();
        services.AddScrapers();
        services.AddProcessors();

        var awsSettings = new AWSSettings();
        builder.Configuration.GetSection(nameof(AWSSettings)).Bind(awsSettings);

        var client = new AmazonCloudWatchLogsClient(new BasicAWSCredentials(awsSettings.AccessKey, awsSettings.SecretKey), Amazon.RegionEndpoint.USEast1);

        configuration
            .WriteTo.AmazonCloudWatch(
                logGroup: awsSettings.LogGroup,
                logStreamPrefix: awsSettings.LogStreamPrefix,
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose,
                createLogGroup: false,
                appendUniqueInstanceGuid: false,
                appendHostName: false,
                logGroupRetentionPolicy: LogGroupRetentionPolicy.OneMonth,
                cloudWatchClient: client);

        return services;
    }

    private static IServiceCollection AddScrapers(this IServiceCollection services)
    {
        services.AddScoped<IScrapingOrchestrator, ScrapingOrchestrator>();

        return services;
    }

    private static IServiceCollection AddProcessors(this IServiceCollection services)
    {
        services.AddScoped<IProcessingOrchestrator, ProcessingOrchestrator>();

        services.ConfigureOptions<GPTOptionsSetup>();

        services.AddTransient<GPTDelegatingHandler>();

        services.AddHttpClient<GPTClient>(
                (serviceProvider, httpClient) =>
                {
                    var geminiOptions = serviceProvider.GetRequiredService<IOptions<GPTOptions>>().Value;

                    httpClient.BaseAddress = new Uri(geminiOptions.Url);
                })
            .AddHttpMessageHandler<GPTDelegatingHandler>();

        return services;
    }
}