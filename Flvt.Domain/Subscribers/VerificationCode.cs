using System.Text;

namespace Flvt.Domain.Subscribers;

public sealed record VerificationCode
{
    public string Code { get; init; }
    public long ExpirationDate { get; init; }

    private VerificationCode(string code, long expirationDate)
    {
        Code = code;
        ExpirationDate = expirationDate;
    }

    internal static VerificationCode Generate()
    {
        var rng = new Random();
        var codeBuilder = new StringBuilder();

        for (int i = 0; i <= 5; i++)
        {
            codeBuilder.Append(rng.Next(0, 10));
        }

        return new VerificationCode(
            codeBuilder.ToString(),
            DateTimeOffset.UtcNow.AddMinutes(60).ToUnixTimeSeconds());
    }

    internal bool Verify(string code) => 
        DateTimeOffset.UtcNow.ToUnixTimeSeconds() < ExpirationDate 
        && 
        Code == code;
}