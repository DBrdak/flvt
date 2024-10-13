using Flvt.Application.Abstractions;
using Flvt.Domain.ProcessedAdvertisements;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Domain.Subscribers;
using Flvt.Infrastructure.Data;
using Flvt.Infrastructure.Data.Repositories;
using Flvt.Infrastructure.Monitoring;
using Flvt.Infrastructure.Processors;
using Flvt.Infrastructure.Processors.AI;
using Flvt.Infrastructure.Processors.AI.GPT;
using Flvt.Infrastructure.Processors.AI.GPT.Options;
using Flvt.Infrastructure.Scrapers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Flvt.Infrastructure;

public static class InfrastructureInjector
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddRepositories();
        services.AddScrapers();
        services.AddProcessors();
        services.AddMonitoring();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<DataContext>();
        services.AddScoped<IProcessedAdvertisementRepository, ProcessedAdvertisementRepository>();
        services.AddScoped<IScrapedAdvertisementRepository, ScrapedAdvertisementRepository>();
        services.AddScoped<ISubscriberRepository, SubscriberRepository>();
        services.AddScoped<BatchRepository>();

        return services;
    }

    private static IServiceCollection AddScrapers(this IServiceCollection services)
    {
        services.AddScoped<IScrapingOrchestrator, ScrapingOrchestrator>();

        return services;
    }

    private static IServiceCollection AddProcessors(this IServiceCollection services)
    {
        services.AddScoped<AIProcessor>();

        services.AddScoped<IProcessingOrchestrator, ProcessingOrchestrator>();

        services.ConfigureOptions<GPTOptionsSetup>();

        services.AddTransient<GPTDelegatingHandler>();

        services.AddHttpClient<GPTClient>(
                (serviceProvider, httpClient) =>
                {
                    var gptOptions = serviceProvider.GetRequiredService<IOptions<GPTOptions>>().Value;

                    httpClient.BaseAddress = new Uri(gptOptions.Url);
                })
            .AddHttpMessageHandler<GPTDelegatingHandler>();

        return services;
    }

    private static IServiceCollection AddMonitoring(this IServiceCollection services)
    {
        services.AddScoped<GPTMonitor>();
        services.AddScoped<ScrapingMonitor>();

        return services;
    }
}