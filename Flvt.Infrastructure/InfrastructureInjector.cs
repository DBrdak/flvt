using Flvt.Application.Abstractions;
using Flvt.Domain.ProcessedAdvertisements;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Domain.Subscribers;
using Flvt.Infrastructure.Custodians;
using Flvt.Infrastructure.Custodians.Assistants;
using Flvt.Infrastructure.Data;
using Flvt.Infrastructure.Data.Repositories;
using Flvt.Infrastructure.Monitoring;
using Flvt.Infrastructure.Processors;
using Flvt.Infrastructure.Processors.AI;
using Flvt.Infrastructure.Processors.AI.GPT;
using Flvt.Infrastructure.Processors.AI.GPT.Options;
using Flvt.Infrastructure.Queues;
using Flvt.Infrastructure.Scrapers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Flvt.Infrastructure;

public static class InfrastructureInjector
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services) =>
        services.AddRepositories()
            .AddScrapers()
            .AddProcessors()
            .AddMonitoring()
            .AddQueues()
            .AddCustody();

    private static IServiceCollection AddQueues(this IServiceCollection services) =>
        services.AddScoped<IQueuePublisher, QueuePublisher>();

    private static IServiceCollection AddCustody(this IServiceCollection services) =>
        services
            .AddScoped<ICustodian, Custodian>()
            .AddScoped<ScrapingCustodialAssistant>()
            .AddScoped<DataCustodialAssistant>();

    private static IServiceCollection AddRepositories(this IServiceCollection services) =>
        services.AddScoped<DataContext>()
            .AddScoped<IProcessedAdvertisementRepository, ProcessedAdvertisementRepository>()
            .AddScoped<IScrapedAdvertisementRepository, ScrapedAdvertisementRepository>()
            .AddScoped<ISubscriberRepository, SubscriberRepository>()
            .AddScoped<BatchRepository>();

    private static IServiceCollection AddScrapers(this IServiceCollection services) => 
        services.AddScoped<IScrapingOrchestrator, ScrapingOrchestrator>();

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
            .AddHttpMessageHandler<GPTDelegatingHandler>()
            .AddDefaultLogger();

        return services;
    }

    private static IServiceCollection AddMonitoring(this IServiceCollection services) =>
        services
            .AddTransient<GPTMonitor>()
            .AddTransient<ScrapingMonitor>();
}