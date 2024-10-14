using Amazon.Lambda.Annotations;
using Flvt.Application.Advertisements.CheckProcessingStatus;
using Flvt.Application.Advertisements.StartProcessingAdvertisements;
using MediatR;
using Serilog.Context;

namespace Flvt.API.Functions.Background;

public sealed class ProcessAdvertisements : BaseFunction
{
    public ProcessAdvertisements(ISender sender) : base(sender)
    {
    }

    [LambdaFunction(ResourceName = $"{nameof(ProcessAdvertisements)}{nameof(StartProcessing)}")]
    public async Task StartProcessing()
    {
        using (LogContext.PushProperty("CorrelationId", Guid.NewGuid()))
        {
            var command = new StartProcessingAdvertisementsCommand();

            _ = await Sender.Send(command);
        }
    }


    [LambdaFunction(ResourceName = $"{nameof(ProcessAdvertisements)}{nameof(EndProcessing)}")]
    public async Task EndProcessing()
    {
        using (LogContext.PushProperty("CorrelationId", Guid.NewGuid()))
        {
            var command = new CheckProcessingResultsCommand();

            _ = await Sender.Send(command);
        }
    }
}