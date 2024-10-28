using Flvt.Application.Behaviors;
using Flvt.Application.Subscribers.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace Flvt.Application;

public static class ApplicationInjector
{
    public static IServiceCollection AddApplication(this IServiceCollection services) =>
        services
            .AddMediatR(
                config =>
                {
                    config.RegisterServicesFromAssemblyContaining<ApplicationAssemblyReference>();
                    config.AddOpenBehavior(typeof(TrackingBehavior<,>));
                })
            .AddLogger()
            .AddServices();

    private static IServiceCollection AddLogger(this IServiceCollection services)
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

    private static IServiceCollection AddServices(this IServiceCollection services) =>
        services
            .AddScoped<FiltersService>();
}