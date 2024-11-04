using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Flvt.Application.Abstractions;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.Subscribers;
using Flvt.Infrastructure.Authentication.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Flvt.Infrastructure.Authentication;

internal sealed class JwtService : IJwtService
{
    private static readonly Error authenticationFailedError = new(
        "Failed to acquire access token do to authentication failure");

    private readonly AuthenticationOptions _options;

    public JwtService(IOptions<AuthenticationOptions> options)
    {
        _options = options.Value;
    }

    public Result<string> GenerateJwt(Subscriber user)
    {
        var claims = UserRepresentationModel
            .FromUser(user)
            .ToClaims();

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: null,
            expires: DateTime.UtcNow.AddMinutes(_options.ExpireInMinutes),
            signingCredentials: signingCredentials);

        var tokenValue = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return tokenValue is null ?
                Result.Failure<string>(authenticationFailedError) :
                Result.Success(tokenValue);
    }

    public bool ValidateJwt(string? token, out string principalId)
    {
        principalId = string.Empty;
        var tokenHandler = new JwtSecurityTokenHandler();
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidIssuer = _options.Issuer,
                ValidAudience = _options.Audience
            };

            _ = tokenHandler.ValidateToken(
                token ?? string.Empty,
                validationParameters,
                out var validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;

            if (IsEmailVerified(jwtToken))
            {
                return false;
            }

            principalId = jwtToken.Subject;

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsEmailVerified(JwtSecurityToken jwtToken) =>
        jwtToken.Claims.FirstOrDefault(
            c => c.Type == UserRepresentationModel.EmailVerifiedClaimName &&
                 c.Value == true.ToString()) is null;
}