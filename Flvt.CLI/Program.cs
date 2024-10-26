using Microsoft.Extensions.DependencyInjection;
using Flvt.Application;
using Flvt.Domain.ProcessedAdvertisements;
using Flvt.Domain.Subscribers;
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
    private readonly IProcessedAdvertisementRepository _repository;

    public Service(ISender sender, IProcessedAdvertisementRepository repository)
    {
        _sender = sender;
        _repository = repository;
    }

    public async Task Run()
    {
        //var a = _repository.GetByFilterAsync(
        //    Subscriber.Register(
        //            "a@a.com",
        //            "PL")
        //        .Value.AddBasicFilter(
        //            "nname",
        //            "Warszawa",
        //            null,
        //            null,
        //            null,
        //            null,
        //            null,
        //            null)
        //        .Value);
        //var cmd = new ScrapeAdvertisementsCommand();
        //var cmd = new StartProcessingAdvertisementsCommand();
        //var cmd = new CheckProcessingStatusCommand();
        //var cmd = new EndProcessingCommand();
        //var cmd = new ProcessAdvertisementsCommand();
        //var cmd = new RemoveOutdatedAdvertisementsCommand();

        //var response = await _sender.Send(cmd);
    }
}

public interface IService
{
    Task Run();
}