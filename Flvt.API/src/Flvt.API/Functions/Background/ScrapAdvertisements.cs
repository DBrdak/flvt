using Amazon.Lambda.Annotations;
using Flvt.Application.Advertisements.ScrapeAdvertisements;
using MediatR;
using Serilog.Context;

namespace Flvt.API.Functions.Background;

public sealed class ScrapAdvertisements : BaseFunction
{
    public ScrapAdvertisements(ISender sender) : base(sender)
    {
    }


    [LambdaFunction(ResourceName = $"{nameof(ScrapAdvertisements)}{nameof(ScrapAll)}")]
    public async Task ScrapAll()
    {
        using (LogContext.PushProperty("CorrelationId", Guid.NewGuid()))
        {
            var command = new ScrapeAdvertisementsCommand();

            _ = await Sender.Send(command);
        }
    }
}
