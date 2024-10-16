using Amazon.Lambda.Annotations;
using Flvt.Application.Advertisements.CheckProcessingStatus;
using Flvt.Application.Advertisements.EndProcessing;
using Flvt.Application.Advertisements.StartProcessingAdvertisements;
using MediatR;

namespace Flvt.API.Functions.Background;

public sealed class ProcessAdvertisements : BaseFunction
{
    public ProcessAdvertisements(ISender sender) : base(sender)
    {
    }

    [LambdaFunction(ResourceName = $"{nameof(ProcessAdvertisements)}{nameof(StartProcessing)}")]
    public async Task StartProcessing()
    {
        var command = new StartProcessingAdvertisementsCommand();

        _ = await Sender.Send(command);
    }

    [LambdaFunction(ResourceName = $"{nameof(ProcessAdvertisements)}{nameof(CheckProcessingStatus)}")]
    public async Task CheckProcessingStatus()
    {
        var command = new CheckProcessingStatusCommand();

        _ = await Sender.Send(command);
    }

    [LambdaFunction(ResourceName = $"{nameof(ProcessAdvertisements)}{nameof(EndProcessing)}")]
    public async Task EndProcessing()
    {
        var command = new EndProcessingCommand();

        _ = await Sender.Send(command);
    }
}