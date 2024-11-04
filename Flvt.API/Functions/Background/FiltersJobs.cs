using System.Runtime.Serialization;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.SQSEvents;
using Flvt.Application.Subscribers.LaunchFilters;
using Flvt.Application.Subscribers.NotifyFilterLaunch;
using MediatR;
using Newtonsoft.Json;
using Serilog;

namespace Flvt.API.Functions.Background;

internal class FiltersJobs : BaseFunction
{
    public FiltersJobs(ISender sender) : base(sender)
    {
    }

    [LambdaFunction(ResourceName = nameof(LaunchFilters))]
    public async Task LaunchFilters()
    {
        var command = new LaunchFiltersCommand();

        _ = await Sender.Send(command);
    }


    [LambdaFunction(ResourceName = nameof(NotifyAboutLaunchedFilters))]
    public async Task NotifyAboutLaunchedFilters(SQSEvent evnt)
    {
        try
        {
            var tasks = evnt.Records.Select(record =>
            {
                var command = new NotifyFilterLaunchCommand(
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
                nameof(NotifyAboutLaunchedFilters),
                e.GetType().Name,
                e.Message);
        }
    }
}