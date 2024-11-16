using Flvt.Domain.Filters;
using Flvt.Domain.Primitives.Advertisements;
using Flvt.Domain.Primitives.Responses;

namespace Flvt.Domain.Subscribers;

public sealed class Subscriber
{
    public Email Email { get; init; }
    public Password Password { get; init; }
    public bool IsEmailVerified { get; private set; }
    public VerificationCode? VerificationCode { get; private set; }
    public LoggingGuard Guard { get; init; }
    public SubscribtionTier Tier { get; init; }
    private readonly List<string> _filtersIds;
    public IReadOnlyList<string> Filters => _filtersIds;


    private Subscriber(
        Email email,
        Password password,
        bool isEmailVerified,
        VerificationCode? verificationCode,
        LoggingGuard guard,
        SubscribtionTier tier,
        List<string> filtersIds)
    {
        Email = email;
        Password = password;
        IsEmailVerified = isEmailVerified;
        VerificationCode = verificationCode;
        Guard = guard;
        Tier = tier;
        _filtersIds = filtersIds;
    }

    public Result<Filter> AddBasicFilter(
        string name,
        string city,
        decimal? minPrice,
        decimal? maxPrice,
        int? minRooms,
        int? maxRooms,
        decimal? minArea,
        decimal? maxArea)
    {
        if (Tier == SubscribtionTier.Basic && _filtersIds.Count >= 1)
        {
            return SubscriberErrors.ActionNotAllowedForBasicSubscribers;
        }

        var filterCreateResult = FilterFactory.CreateBasicFilter(
            name,
            city,
            minPrice,
            maxPrice,
            minRooms,
            maxRooms,
            minArea,
            maxArea,
            this);

        if (filterCreateResult.IsFailure)
        {
            return filterCreateResult.Error;
        }

        var filter = filterCreateResult.Value;
        
        _filtersIds.Add(filter.Id);

        return Result.Success(filter);
    }

    public Result<string> GetFilter(string filterId)
    {
        var filter = _filtersIds.FirstOrDefault(id => id == filterId);

        return filter is null ?
            SubscriberErrors.FilterNotFound :
            filter;
    }

    public static Result<Subscriber> Register(string email, string passwordPlainText)
    {
        var emailResult = Email.Create(email);

        if (emailResult.IsFailure)
        {
            return emailResult.Error;
        }

        var passwordCreateResult = Subscribers.Password.Create(passwordPlainText);

        if (passwordCreateResult.IsFailure)
        {
            return passwordCreateResult.Error;
        }

        var password = passwordCreateResult.Value;

        var subscriber = new Subscriber(
            emailResult.Value,
            password,
            false,
            null,
            LoggingGuard.Create(),
            SubscribtionTier.Basic,
            []);

        subscriber.GenerateVerificationCode();

        return subscriber;
    }

    public Result LogIn(string passwordPlainText)
    {
        var passwordVerifyResult = Password.VerifyPassword(passwordPlainText);

        if (passwordVerifyResult.IsFailure)
        {
            Guard.LogInFailed();
            return Result.Failure(passwordVerifyResult.Error);
        }

        if (Guard.IsLocked)
        {
            return SubscriberErrors.UserLocked(Guard.RemainingLockSeconds);
        }

        Guard.LoginSucceeded();

        return Result.Success();
    }

    public Result VerifyEmail(string verificationCode)
    {
        var verificationCodeResult = VerifyCode(verificationCode);

        if (verificationCodeResult.IsFailure)
        {
            return verificationCodeResult.Error;
        }

        IsEmailVerified = true;

        return Result.Success();
    }

    private VerificationCode GenerateVerificationCode()
    {
        var code = VerificationCode.Generate();

        VerificationCode = code;

        return code;
    }


    private Result VerifyCode(string code)
    {
        var isSuccess = VerificationCode?.Verify(code) ?? false;

        VerificationCode = null;

        return isSuccess ?
            Result.Success() :
            Result.Failure(SubscriberErrors.VerificationCodeInvalid);
    }

    public Result<string> RequestNewPassword()
    {
        if (!IsEmailVerified)
        {
            return SubscriberErrors.EmailNotVerified;
        }

        return GenerateVerificationCode().Code;
    }

    public Result ChangePassword(string newPlainPassword, string verificationCode)
    {
        var verificationResult = VerifyCode(verificationCode);

        if (verificationResult.IsFailure)
        {
            return verificationResult.Error;
        }

        VerificationCode = null;

        var newPasswordCreateResult = Password.Create(newPlainPassword);

        return newPasswordCreateResult;
    }

    public void ReGenerateVerificationCode()
    {
        var code = VerificationCode.Generate();

        VerificationCode = code;
    }

    public Result<string> RemoveFilter(
        string filterId) =>
        _filtersIds.Remove(filterId) ?
            filterId :
            SubscriberErrors.FilterNotFound;
}