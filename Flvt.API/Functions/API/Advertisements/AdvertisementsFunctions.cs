using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.APIGatewayEvents;
using Flvt.API.Utils;
using Flvt.Application.Advertisements.GetAllAdvertisements;
using Flvt.Application.Advertisements.Models;
using Flvt.Domain.Primitives.Responses;
using MediatR;
using Serilog;

namespace Flvt.API.Functions.API.Advertisements;

public sealed class AdvertisementsFunctions : BaseFunction
{
    public AdvertisementsFunctions(ISender sender) : base(sender)
    {
    }


    [LambdaFunction(ResourceName = $"Advertisements{nameof(GetAllAdvertisements)}")]
    [HttpApi(LambdaHttpMethod.Get, "/v1/advertisements")]
    public async Task<Result<IEnumerable<ProcessedAdvertisementModel>>> GetAllAdvertisements()
    {
        var query = new GetAllAdvertisementsQuery();

        var result = await Sender.Send(query);
        
        //return result.ReturnAPIResponse(200, 404);
        return result;
    }
}