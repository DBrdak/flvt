using Amazon.Lambda.APIGatewayEvents;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Flvt.API.Utils;

internal static class RequestContextExtensions
{
    public static string GetJwtClaimValue(this APIGatewayHttpApiV2ProxyRequest requestContext, string claimName)
    {
        var authorizationHeader = requestContext.Headers.FirstOrDefault(header => header.Key.ToLower() == "authorization");

        var encodedJwt = authorizationHeader.Value.Split(' ').Last();

        var jwt = new JsonWebToken(encodedJwt);

        return jwt.Claims
                   .FirstOrDefault(claim => claim.Type.ToLower() == claimName.ToLower())?
                   .Value ??
               string.Empty;
    }
}