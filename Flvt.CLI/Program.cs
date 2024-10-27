using Microsoft.Extensions.DependencyInjection;
using Flvt.Application;
using Flvt.Application.Abstractions;
using Flvt.Application.Advertisements.StartProcessingAdvertisements;
using Flvt.Domain.Photos;
using Flvt.Domain.ProcessedAdvertisements;
using Flvt.Domain.ScrapedAdvertisements;
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
    private readonly IScrapedAdvertisementRepository _scrapedAdvertisementRepository;
    private readonly IAdvertisementPhotosRepository _advertisementPhotosRepository;
    private readonly IScrapingOrchestrator _scrapingOrchestrator;

    public Service(
        ISender sender,
        IProcessedAdvertisementRepository repository,
        IScrapedAdvertisementRepository scrapedAdvertisementRepository,
        IAdvertisementPhotosRepository advertisementPhotosRepository,
        IScrapingOrchestrator scrapingOrchestrator)
    {
        _sender = sender;
        _repository = repository;
        _scrapedAdvertisementRepository = scrapedAdvertisementRepository;
        _advertisementPhotosRepository = advertisementPhotosRepository;
        _scrapingOrchestrator = scrapingOrchestrator;
    }

    public async Task Run()
    {
        //var a = _repository.GetByFilterAsync(
        //    Subscriber.Register(
        //            "a@a.com",
        //            "PL")
        //        .Amount.AddBasicFilter(
        //            "nname",
        //            "Warszawa",
        //            null,
        //            null,
        //            null,
        //            null,
        //            null,
        //            null)
        //        .Amount);
        //var cmd = new ScrapeAdvertisementsCommand();
        var cmd = new StartProcessingAdvertisementsCommand();
        //var cmd = new CheckProcessingStatusCommand();
        //var cmd = new EndProcessingCommand();
        //var cmd = new ProcessAdvertisementsCommand();
        //var cmd = new RemoveOutdatedAdvertisementsCommand();

        var response = await _sender.Send(cmd);
    }
}

public interface IService
{
    Task Run();
}