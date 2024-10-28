using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.APIGatewayEvents;
using Flvt.API.Utils;
using Flvt.Application.Subscribers.Login;
using Flvt.Application.Subscribers.Register;
using Flvt.Application.Subscribers.ResendEmail;
using Flvt.Application.Subscribers.VerifyEmail;
using Flvt.Infrastructure.Authentication.Models;
using MediatR;

namespace Flvt.API.Functions.API.Auth;

internal sealed class AuthFunctions : BaseFunction
{
    public AuthFunctions(ISender sender) : base(sender)
    {
    }

    [LambdaFunction(ResourceName = nameof(Register))]
    [HttpApi(LambdaHttpMethod.Post, "/v1/auth/register")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> Register(
        [FromBody] RegisterCommand command,
        APIGatewayHttpApiV2ProxyRequest request)
    {
        var result = await Sender.Send(command);

        return result.ReturnAPIResponse(200, 400);
    }

    [LambdaFunction(ResourceName = nameof(Login))]
    [HttpApi(LambdaHttpMethod.Post, "/v1/auth/login")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> Login(
        [FromBody] LoginCommand command,
        APIGatewayHttpApiV2ProxyRequest request)
    {
        var result = await Sender.Send(command);

        return result.ReturnAPIResponse(200, 401);
    }

    [LambdaFunction(ResourceName = nameof(VerifyEmail))]
    [HttpApi(LambdaHttpMethod.Post, "/v1/auth/verify")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> VerifyEmail(
        [FromQuery] string verificationCode,
        APIGatewayHttpApiV2ProxyRequest request)
    {
        var subscriberEmail = request
            .RequestContext
            .Authorizer
            .Jwt
            .Claims
            .FirstOrDefault(kvp => kvp.Key == UserRepresentationModel.EmailClaimName)
            .Value;

        var command = new VerifyEmailCommand(subscriberEmail, verificationCode);

        var result = await Sender.Send(command);

        return result.ReturnAPIResponse(200, 400);
    }

    [LambdaFunction(ResourceName = nameof(ResendVerificationEmail))]
    [HttpApi(LambdaHttpMethod.Put, "/v1/auth/resend")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> ResendVerificationEmail(
        [FromQuery] string purpose,
        APIGatewayHttpApiV2ProxyRequest request)
    {
        var subscriberEmail = request
            .RequestContext
            .Authorizer
            .Jwt
            .Claims
            .FirstOrDefault(kvp => kvp.Key == UserRepresentationModel.EmailClaimName)
            .Value;

        var command = new ResendEmailCommand(subscriberEmail, purpose);

        var result = await Sender.Send(command);

        return result.ReturnAPIResponse(200, 400);
    }
}