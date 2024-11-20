using Flvt.Application.Abstractions;
using Flvt.Domain.Filters;
using Flvt.Domain.Photos;
using Flvt.Domain.ProcessedAdvertisements;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Domain.Subscribers;
using Flvt.Infrastructure.Authentication;
using Flvt.Infrastructure.Custodians;
using Flvt.Infrastructure.Custodians.Assistants;
using Flvt.Infrastructure.Data;
using Flvt.Infrastructure.Data.DataModels;
using Flvt.Infrastructure.Data.Repositories;
using Flvt.Infrastructure.FileBucket;
using Flvt.Infrastructure.Messanger.Emails;
using Flvt.Infrastructure.Messanger.Emails.Resend;
using Flvt.Infrastructure.Monitoring;
using Flvt.Infrastructure.Processors;
using Flvt.Infrastructure.Processors.AI;
using Flvt.Infrastructure.Processors.AI.GPT;
using Flvt.Infrastructure.Processors.AI.GPT.Options;
using Flvt.Infrastructure.Queues;
using Flvt.Infrastructure.Scrapers;
using Flvt.Infrastructure.Scrapers.Shared.Helpers.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Flvt.Infrastructure;

public static class InfrastructureInjector
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services) =>
        services.AddDataAccessLayer()
            .AddScrapers()
            .AddProcessors()
            .AddMonitoring()
            .AddQueues()
            .AddCustody()
            .AddFiles()
            .AddAuthentication()
            .AddEmails();

    private static IServiceCollection AddQueues(this IServiceCollection services) =>
        services.AddScoped<IQueuePublisher, QueuePublisher>();

    private static IServiceCollection AddCustody(this IServiceCollection services) =>
        services
            .AddScoped<ICustodian, Custodian>()
            .AddScoped<ScrapingCustodialAssistant>()
            .AddScoped<DataCustodialAssistant>();

    private static IServiceCollection AddDataAccessLayer(this IServiceCollection services) =>
        services.AddScoped<DataContext>()
            .AddScoped(typeof(DataModelService<>))
            .AddScoped<IAdvertisementPhotosRepository, AdvertisementPhotosRepository>()
            .AddScoped<IProcessedAdvertisementRepository, ProcessedAdvertisementRepository>()
            .AddScoped<IScrapedAdvertisementRepository, ScrapedAdvertisementRepository>()
            .AddScoped<ISubscriberRepository, SubscriberRepository>()
            .AddScoped<IFilterRepository, FilterRepository>()
            .AddScoped<IScraperHelperRepository, ScraperHelperRepository>()
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

    private static IServiceCollection AddFiles(this IServiceCollection services) => 
        services
            .AddScoped<S3Bucket>()
            .AddScoped<IFileService, FileService>();

    private static IServiceCollection AddMonitoring(this IServiceCollection services) =>
        services
            .AddTransient<GPTMonitor>()
            .AddTransient<AdsScrapingMonitor>();

    private static IServiceCollection AddAuthentication(this IServiceCollection services) =>
        services
            .ConfigureOptions<AuthenticationOptionsSetup>()
            .ConfigureOptions<JwtBearerOptionsSetup>()
            .AddScoped<IJwtService, JwtService>()
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer()
            .Services;

    private static IServiceCollection AddEmails(this IServiceCollection services) =>
        services
            .AddScoped<IEmailService, EmailService>()
            .AddScoped<ResendClient>();
}