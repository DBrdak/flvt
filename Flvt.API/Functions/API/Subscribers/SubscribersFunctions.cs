using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.APIGatewayEvents;
using Flvt.API.Utils;
using Flvt.Application.Subscribers.GetSubscriber;
using MediatR;

namespace Flvt.API.Functions.API.Subscribers;

public sealed class SubscribersFunctions : BaseFunction
{
    public SubscribersFunctions(ISender sender) : base(sender)
    {
    }

    [LambdaFunction(ResourceName = nameof(GetSubscriber))]
    [HttpApi(LambdaHttpMethod.Get, "/v1/subscribers")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> GetSubscriber(APIGatewayHttpApiV2ProxyRequest request)
    {
        var subscriberEmail = request
            .RequestContext
            .Authorizer
            .Jwt
            .Claims
            .FirstOrDefault(kvp => kvp.Key.ToLower() == "email")
            .Value;

        var query = new GetSubscriberQuery(subscriberEmail);

        var result = await Sender.Send(query);

        return result.ReturnAPIResponse(200, 404);
    }
}