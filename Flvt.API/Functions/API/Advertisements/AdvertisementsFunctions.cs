using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.APIGatewayEvents;
using Flvt.API.Utils;
using Flvt.Application.Advertisements.GetAdvertisementsByFilter;
using MediatR;

namespace Flvt.API.Functions.API.Advertisements;

public sealed class AdvertisementsFunctions : BaseFunction
{
    public AdvertisementsFunctions(ISender sender) : base(sender)
    {
    }

    [LambdaFunction(ResourceName = $"Advertisements{nameof(GetByFilter)}")]
    [HttpApi(LambdaHttpMethod.Get, "/v1/advertisements")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> GetByFilter(
        [FromQuery] string? filterId,
        APIGatewayHttpApiV2ProxyRequest request)
    {
        var subscriberEmail = request
            .RequestContext
            .Authorizer
            .Jwt
            .Claims
            .FirstOrDefault(kvp => kvp.Key.ToLower() == "email")
            .Value;

        var query = new GetAdvertisementsByFilterQuery(
            filterId,
            subscriberEmail);

        var result = await Sender.Send(query);

        return result.ReturnAPIResponse(200, 404);
    }
}