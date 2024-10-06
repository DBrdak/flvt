﻿using Amazon.CloudWatchLogs;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Sinks.AwsCloudWatch;
using Serilog.Sinks.AwsCloudWatch.LogStreamNameProvider;

namespace Flvt.Application;

public static class ApplicationInjector
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(
            config =>
            {
                config.RegisterServicesFromAssemblyContaining<ApplicationAssemblyReference>();
            });

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
            .CreateLogger();

        return services;
    }
}