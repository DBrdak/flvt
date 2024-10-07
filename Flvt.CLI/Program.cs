using Flvt.Application.ProcessAdvertisements;
using Flvt.Domain.Subscribers;
using Microsoft.Extensions.DependencyInjection;
using Flvt.Application;
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
        var filter = new Filter()
            .InLocation("warszawa")
            .ToArea(45)
            .FromArea(30)
            .ToPrice(2800)
            .FromRooms(2)
            .ToRooms(4)
            .Build();
        var cmd = new ProcessAdvertisementsCommand(
            filter.Location,
            filter.MinPrice,
            filter.MaxPrice,
            filter.MinRooms,
            filter.MaxRooms,
            filter.MinArea,
            filter.MaxArea);

        var response = await _sender.Send(cmd);
    }
}

public interface IService
{
    Task Run();
}