﻿using Amazon.Lambda.Annotations;
using Flvt.Application.Advertisements.StartProcessingAdvertisements;
using Flvt.Application.Custody.RemoveDuplicateAdvertisements;
using Flvt.Application.Custody.RemoveOutdatedAdvertisements;
using MediatR;

namespace Flvt.API.Functions.Background;

internal class CustodyFunctions : BaseFunction
{
    public CustodyFunctions(ISender sender) : base(sender)
    {
    }

    [LambdaFunction(ResourceName = $"{nameof(CustodyFunctions)}{nameof(RemoveOutdated)}")]
    public async Task RemoveOutdated()
    {
        var command = new RemoveOutdatedAdvertisementsCommand();

        _ = await Sender.Send(command);
    }

    [LambdaFunction(ResourceName = $"{nameof(CustodyFunctions)}{nameof(RemoveDuplicates)}")]
    public async Task RemoveDuplicates()
    {
        var command = new RemoveDuplicateAdvertisementsCommand();

        _ = await Sender.Send(command);
    }
}