using Amazon.Lambda.Annotations;
using Flvt.Application.Advertisements.ScrapeAdvertisements;
using MediatR;

namespace Flvt.API.Functions.Background;

public sealed class ScrapeAdvertisements : BaseFunction
{
    public ScrapeAdvertisements(ISender sender) : base(sender)
    {
    }


    [LambdaFunction(ResourceName = $"{nameof(ScrapeAdvertisements)}{nameof(ScrapAll)}")]
    public async Task ScrapAll()
    {
        var command = new ScrapeAdvertisementsCommand();

        _ = await Sender.Send(command);
    }
}
