using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.Subscribers;

namespace Flvt.Application.Abstractions;

public interface IJwtService
{
    Result<string> GenerateJwt(Subscriber user);

    public bool ValidateJwt(string? token, bool shouldValidateEmail, out string principalId);
}