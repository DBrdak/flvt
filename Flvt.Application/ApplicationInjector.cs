using Flvt.Application.Behaviors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Formatting.Json;

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

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Seq(config["seq:uri"])
            .WriteTo.Console()
            .CreateLogger();

        return services;
    }
}