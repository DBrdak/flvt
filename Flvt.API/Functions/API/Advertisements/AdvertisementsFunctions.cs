using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.APIGatewayEvents;
using Flvt.API.Utils;
using Flvt.Application.Advertisements.Flag;
using Flvt.Application.Advertisements.Follow;
using Flvt.Application.Advertisements.GetAdvertisementsByFilter;
using Flvt.Application.Advertisements.MarkAsSeen;
using Flvt.Domain.Primitives.Responses;
using Flvt.Infrastructure.Authentication.Models;
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
        var subscriberEmail = request.GetJwtClaimValue(UserRepresentationModel.EmailClaimName);

        var query = new GetAdvertisementsByFilterQuery(
            filterId,
            subscriberEmail);

        var result = await Sender.Send(query) ?? Error.Exception;

        return result.ReturnAPIResponse(200, 404);
    }

    [LambdaFunction(ResourceName = $"Advertisements{nameof(MarkAsSeen)}")]
    [HttpApi(LambdaHttpMethod.Put, "/v1/advertisements/see")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> MarkAsSeen(
        [FromQuery] string advertisementLink,
        [FromQuery] string filterId)
    {
        var command = new MarkAsSeenCommand(filterId, advertisementLink);

        var result = await Sender.Send(command) ?? Error.Exception;

        return result.ReturnAPIResponse(200, 400);
    }

    [LambdaFunction(ResourceName = $"Advertisements{nameof(Follow)}")]
    [HttpApi(LambdaHttpMethod.Put, "/v1/advertisements/follow")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> Follow(
        [FromQuery] string advertisementLink,
        [FromQuery] string filterId)
    {
        var command = new FollowCommand(filterId, advertisementLink);

        var result = await Sender.Send(command) ?? Error.Exception;

        return result.ReturnAPIResponse(200, 400);
    }

    [LambdaFunction(ResourceName = $"Advertisements{nameof(Flag)}")]
    [HttpApi(LambdaHttpMethod.Put, "/v1/advertisements/flag")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> Flag(
        [FromQuery] string advertisementLink)
    {
        var command = new FlagCommand(advertisementLink);

        var result = await Sender.Send(command) ?? Error.Exception;

        return result.ReturnAPIResponse(200, 400);
    }
}