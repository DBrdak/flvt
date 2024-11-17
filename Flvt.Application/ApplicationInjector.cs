using Flvt.Application.Behaviors;
using Flvt.Application.Subscribers.Services;
using Microsoft.Extensions.DependencyInjection;

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
            .AddServices();

    private static IServiceCollection AddServices(this IServiceCollection services) =>
        services
            .AddScoped<FiltersService>();
}