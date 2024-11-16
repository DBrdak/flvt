namespace Flvt.Infrastructure.Authentication;

public sealed class AuthenticationOptions
{
    public string Audience { get; init; } = "Flvt";

    public int ExpireInMinutes { get; init; } = 1_440; // day

    public string SecretKey { get; set; } = string.Empty;

    public string Issuer { get; set; } = "Flvt";
}