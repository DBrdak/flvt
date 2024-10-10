using Microsoft.Extensions.DependencyInjection;
using Flvt.Application;
using Flvt.Application.Advertisements.ScrapeAdvertisements;
using Flvt.Infrastructure;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Flvt.CLI;

internal class Program
{
        
    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .Build();

        var serviceProvider = new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration)
            .AddScoped<IService, Service>()
            .AddApplication()
            .AddInfrastructure()
            .BuildServiceProvider();

        var s = serviceProvider.GetRequiredService<IService>();

        await s.Run();
    }
}

public class Service : IService
{
    private readonly ISender _sender;

    public Service(ISender sender)
    {
        _sender = sender;
    }

    public async Task Run()
    {
        //var cmd = new ProcessAdvertisementsCommand(
        //    "example",
        //    "warszawa",
        //    0,
        //    3000,
        //    2,
        //    2,
        //    30,
        //    50);

        var cmd = new ScrapeAdvertisementsCommand();

        var response = await _sender.Send(cmd);
    }
}

public interface IService
{
    Task Run();
}