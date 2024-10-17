﻿using Microsoft.Extensions.DependencyInjection;
using Flvt.Application;
using Flvt.Application.Advertisements.CheckProcessingStatus;
using Flvt.Application.Advertisements.EndProcessing;
using Flvt.Application.Advertisements.ScrapeAdvertisements;
using Flvt.Application.Advertisements.StartProcessingAdvertisements;
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
        //var cmd = new ScrapeAdvertisementsCommand();
        var cmd = new StartProcessingAdvertisementsCommand();
        //var cmd = new CheckProcessingStatusCommand();
        //var cmd = new EndProcessingCommand();

        var response = await _sender.Send(cmd);
    }
}

public interface IService
{
    Task Run();
}