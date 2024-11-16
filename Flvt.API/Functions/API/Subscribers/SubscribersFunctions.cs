using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.APIGatewayEvents;
using Flvt.API.Functions.API.Subscribers.Requests;
using Flvt.API.Utils;
using Flvt.Application.Subscribers.AddBasicFilter;
using Flvt.Application.Subscribers.GetSubscriber;
using Flvt.Application.Subscribers.RemoveFilter;
using Flvt.Domain.Primitives.Responses;
using Flvt.Infrastructure.Authentication.Models;
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
        var subscriberEmail = request.GetJwtClaimValue(UserRepresentationModel.EmailClaimName);

        var query = new GetSubscriberQuery(subscriberEmail);

        var result = await Sender.Send(query) ?? Error.Exception;

        return result.ReturnAPIResponse(200, 404);
    }

    [LambdaFunction(ResourceName = nameof(AddBasicFilter))]
    [HttpApi(LambdaHttpMethod.Post, "/v1/subscribers/filters/basic")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> AddBasicFilter(
        [FromBody] AddBasicFilterRequest request,
        APIGatewayHttpApiV2ProxyRequest requestContext)
    {
        var subscriberEmail = requestContext.GetJwtClaimValue(UserRepresentationModel.EmailClaimName);

        var query = new AddBasicFilterCommand(
            subscriberEmail,
            request.Name,
            request.City,
            request.MinPrice,
            request.MaxPrice,
            request.MinRooms,
            request.MaxRooms,
            request.MinArea,
            request.MaxArea);

        var result = await Sender.Send(query) ?? Error.Exception;

        return result.ReturnAPIResponse(200, 400);
    }

    [LambdaFunction(ResourceName = nameof(RemoveFilter))]
    [HttpApi(LambdaHttpMethod.Delete, "/v1/subscribers/filters/{filterId}")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> RemoveFilter(
        string filterId,
        APIGatewayHttpApiV2ProxyRequest request)
    {
        var subscriberEmail = request.GetJwtClaimValue(UserRepresentationModel.EmailClaimName);

        var query = new RemoveFilterCommand(subscriberEmail, filterId);

        var result = await Sender.Send(query) ?? Error.Exception;

        return result.ReturnAPIResponse(200, 400);
    }
}