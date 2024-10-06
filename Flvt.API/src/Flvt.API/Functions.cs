using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Flvt.API.Utils;
using Flvt.Application.ProcessAdvertisements;
using Flvt.Domain.Advertisements;
using MediatR;
using Serilog;
using Serilog.Context;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Flvt.API;

public class Functions
{
    private readonly ISender _sender;

    public Functions(ISender sender)
    {
        _sender = sender;
    }

    [HttpApi(LambdaHttpMethod.Post, "v1")]
    [LambdaFunction(ResourceName = nameof(ProcessedAdvertisement))]
    public async Task<APIGatewayHttpApiV2ProxyResponse> ProcessNewAdvertisements(
        [FromBody] string emailAddress,
        [FromQuery] string location,
        [FromQuery] int? minPrice,
        [FromQuery] int? maxPrice,
        [FromQuery] int? minRooms,
        [FromQuery] int? maxRooms,
        [FromQuery] int? minArea,
        [FromQuery] int? maxArea,
        APIGatewayHttpApiV2ProxyRequest requestContext)
    {
        using (LogContext.PushProperty("CorrelationId", requestContext.RequestContext.RequestId))
        {
            var command = new ProcessAdvertisementsCommand(
                location,
                minPrice,
                maxPrice,
                minRooms,
                maxRooms,
                minArea,
                maxArea);

            var result = await _sender.Send(command);

            return result.ReturnAPIResponse();
        }
    }

    [LambdaFunction(ResourceName = nameof(ProcessAdvertisementsForUsers))]
    public async Task<APIGatewayHttpApiV2ProxyResponse> ProcessAdvertisementsForUsers()
    {
        using (LogContext.PushProperty("CorrelationId", Guid.NewGuid()))
        {
            //var command = new ProcessAdvertisementsCommand();

            //var result = await _sender.Send(command);

            //return result.ReturnAPIResponse();
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 200,
                Body = "Hello World",
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "text/plain" }
                }
            };
        }
    }
}