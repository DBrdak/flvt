using Flvt.Domain.Primitives.Responses;

namespace Flvt.Domain.Subscribers;
public sealed record LoggingGuard
{
    public int LoginAttempts { get; private set; }
    public long? LockedUntil { get; private set; }
    internal bool IsLocked => DateTimeOffset.UtcNow.ToUnixTimeSeconds() < LockedUntil;
    internal int RemainingLockSeconds => LockedUntil is null ?
        0 :
        (int)(LockedUntil - DateTimeOffset.UtcNow.ToUnixTimeSeconds());

    private static readonly Dictionary<int, int> userLockPolicy = new()
    {
        { 5, 1 },
        { 10, 2 },
        { 15, 3 },
    };

    private LoggingGuard(int loginAttempts, long? lockedUntil)
    {
        LoginAttempts = loginAttempts;
        LockedUntil = lockedUntil;
    }

    internal static LoggingGuard Create() => new(0, null);

    internal Result LogInFailed()
    {
        LoginAttempts++;

        switch (LoginAttempts)
        {
            case var x when x % 5 == 0 && x > 10:
                LockedUntil = DateTimeOffset.UtcNow.AddMinutes(userLockPolicy[15]).ToUnixTimeSeconds();
                return SubscriberErrors.UserLocked(userLockPolicy[15] * 60);
            case 10:
                LockedUntil = DateTimeOffset.UtcNow.AddMinutes(userLockPolicy[10]).ToUnixTimeSeconds();
                return SubscriberErrors.UserLocked(userLockPolicy[10] * 60);
            case 5:
                LockedUntil = DateTimeOffset.UtcNow.AddMinutes(userLockPolicy[5]).ToUnixTimeSeconds();
                return SubscriberErrors.UserLocked(userLockPolicy[5] * 60);
        }

        return Result.Success();
    }

    internal void LoginSucceeded() => (LoginAttempts, LockedUntil) = (0, null);
}
