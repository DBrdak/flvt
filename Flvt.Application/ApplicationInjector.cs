using Flvt.Application.Behaviors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

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
                .WriteTo.Console()
                .CreateLogger(),
            _ => new LoggerConfiguration()
                .WriteTo.Console()
                .Enrich.FromLogContext()
                .CreateLogger()
        };

        return services;
    }
}