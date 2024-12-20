﻿using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.APIGatewayEvents;
using Flvt.API.Utils;
using Flvt.Application.Subscribers.Login;
using Flvt.Application.Subscribers.Register;
using Flvt.Application.Subscribers.RequestNewPassword;
using Flvt.Application.Subscribers.ResendEmail;
using Flvt.Application.Subscribers.SetNewPassword;
using Flvt.Application.Subscribers.VerifyEmail;
using Flvt.Domain.Primitives.Responses;
using MediatR;
using Serilog;

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
        Log.Logger.Information("Registering subscriber with email {email}", command.Email);

        var result = await Sender.Send(command) ?? Error.Exception;

        return result.ReturnAPIResponse(200, 400);
    }

    [LambdaFunction(ResourceName = nameof(Login))]
    [HttpApi(LambdaHttpMethod.Post, "/v1/auth/login")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> Login(
        [FromBody] LoginCommand command,
        APIGatewayHttpApiV2ProxyRequest request)
    {
        var result = await Sender.Send(command) ?? Error.Exception;

        return result.ReturnAPIResponse(200, 400);
    }

    [LambdaFunction(ResourceName = nameof(VerifyEmail))]
    [HttpApi(LambdaHttpMethod.Post, "/v1/auth/verify")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> VerifyEmail(
        [FromBody] VerifyEmailCommand command,
        APIGatewayHttpApiV2ProxyRequest request)
    {
        var result = await Sender.Send(command) ?? Error.Exception;

        return result.ReturnAPIResponse(200, 400);
    }

    [LambdaFunction(ResourceName = nameof(ResendEmail))]
    [HttpApi(LambdaHttpMethod.Put, "/v1/auth/resend")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> ResendEmail(
        [FromQuery] string purpose,
        [FromQuery] string email,
        APIGatewayHttpApiV2ProxyRequest request)
    {
        var command = new ResendEmailCommand(email, purpose);

        var result = await Sender.Send(command) ?? Error.Exception;

        return result.ReturnAPIResponse(200, 400);
    }

    [LambdaFunction(ResourceName = nameof(RequestNewPassword))]
    [HttpApi(LambdaHttpMethod.Put, "/v1/auth/new-password/request")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> RequestNewPassword(
        [FromQuery] string email,
        APIGatewayHttpApiV2ProxyRequest request)
    {
        var command = new RequestNewPasswordCommand(email);

        var result = await Sender.Send(command) ?? Error.Exception;

        return result.ReturnAPIResponse(200, 400);
    }

    [LambdaFunction(ResourceName = nameof(SetNewPassword))]
    [HttpApi(LambdaHttpMethod.Post, "/v1/auth/new-password/set")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> SetNewPassword(
        [FromBody] SetNewPasswordCommand command,
        APIGatewayHttpApiV2ProxyRequest requestContext)
    {
        var result = await Sender.Send(command) ?? Error.Exception;

        return result.ReturnAPIResponse(200, 400);
    }

}