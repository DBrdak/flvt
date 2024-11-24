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
    public async Task ScrapeAds(SQSEvent evnt)
    {
        try
        {
            var tasks = evnt.Records.Select(record =>
            {
                var command = new ScrapeAdvertisementsCommand(
                    JsonConvert.DeserializeObject<IEnumerable<string>>(record.Body) ??
                    throw new SerializationException("Failed to deserialize SQS event to Filters IDs array"));

                return Sender.Send(command);
            });

            await Task.WhenAll(tasks);
        }
        catch (Exception e)
        {
            Log.Logger.Error(
                "Problem occured when trying to execute {queueHandlerName} Lambda queue handler" +
                "Exception: {exception}" +
                "Details: {message}",
                nameof(ScrapeAds),
                e.GetType().Name,
                e.Message);
        }
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
