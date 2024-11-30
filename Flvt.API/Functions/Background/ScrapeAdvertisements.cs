using Amazon.Lambda.Annotations;
using Amazon.Lambda.SQSEvents;
using Flvt.Application.Advertisements.ScrapeAdvertisements;
using Flvt.Application.Advertisements.ScrapeAdvertisementsLinks;
using MediatR;
using Newtonsoft.Json;
using Serilog;
using System.Runtime.Serialization;
using Amazon.EventBridge;
using Amazon.Lambda.Core;

namespace Flvt.API.Functions.Background;

public sealed class ScrapeAdvertisements : BaseFunction
{
    public ScrapeAdvertisements(ISender sender) : base(sender)
    {
    }


    [LambdaFunction(ResourceName = nameof(ScrapeAds))]
    public async Task ScrapeAds()
    {
        var command = new ScrapeAdvertisementsCommand();

        _ = await Sender.Send(command);
    }

    [LambdaFunction(ResourceName = nameof(ScrapeLinks))]
    public async Task ScrapeLinks(Dictionary<string, string> input)
    {
        var service = input?.GetValueOrDefault("Service");

        if (string.IsNullOrWhiteSpace(service))
        {
            Log.Logger.Error("Service name is not provided");
            return;
        }

        var command = new ScrapeAdvertisementsLinksCommand(service);

        _ = await Sender.Send(command);
    }
}
