using Amazon.Lambda.Annotations;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Flvt.Application.Abstractions;
using MediatR;

namespace Flvt.API.Functions.API.Auth;

internal class AuthorizerFunctions : BaseFunction
{
    private readonly IJwtService _jwtService;

    public AuthorizerFunctions(
        IJwtService jwtService,
        ISender sender) : base(sender)
    {
        _jwtService = jwtService;
    }

    [LambdaFunction(ResourceName = "FlvtAuthorizer")]
    public APIGatewayCustomAuthorizerV2SimpleResponse FunctionHandler(
        APIGatewayCustomAuthorizerV2Request request,
        ILambdaContext context)
    {
        var token = request.Headers
            .FirstOrDefault(kvp => kvp.Key.ToLower() == "authorization").Value?
            .Replace("Bearer ", string.Empty);

        var isAuthorized = _jwtService.ValidateJwt(token, out var principalId);

        return new APIGatewayCustomAuthorizerV2SimpleResponse
        {
            IsAuthorized = isAuthorized
        };
    }
}