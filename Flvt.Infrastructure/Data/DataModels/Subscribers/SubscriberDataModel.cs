using Amazon.DynamoDBv2.DocumentModel;
using Flvt.Domain.Primitives.Advertisements;
using Flvt.Domain.Subscribers;
using Flvt.Infrastructure.Data.DataModels.Exceptions;
using System.Reflection;
using Flvt.Infrastructure.Data.Extensions;

namespace Flvt.Infrastructure.Data.DataModels.Subscribers;

internal sealed class SubscriberDataModel : IDataModel<Subscriber>
{
    public string Email { get; init; }
    public string Password { get; init; }
    public bool IsEmailVerified { get; init; }
    public string? VerificationCode { get; init; }
    public long? VerificationCodeExpirationDate { get; init; }
    public int LoggingGuardLoginAttempts { get; init; }
    public long? LoggingGuardLockedUntil { get; init; }
    public string Tier { get; init; }
    public IEnumerable<string> Filters { get; init; }

    public SubscriberDataModel(Subscriber original)
    {
        Email = original.Email.Value;
        Password = original.Password.Hash;
        IsEmailVerified = original.IsEmailVerified;
        VerificationCode = original.VerificationCode?.Code;
        VerificationCodeExpirationDate = original.VerificationCode?.ExpirationDate ?? 0;
        LoggingGuardLoginAttempts = original.Guard.LoginAttempts;
        LoggingGuardLockedUntil = original.Guard.LockedUntil;
        Tier = original.Tier.Value;
        Filters = original.Filters;
    }
    public SubscriberDataModel(Document doc)
    {
        Email = doc.GetProperty(nameof(Email));
        Password = doc.GetProperty(nameof(Password));
        IsEmailVerified = doc.GetProperty(nameof(IsEmailVerified)).AsBoolean();
        VerificationCode = doc.GetNullableProperty(nameof(VerificationCode))?.AsNullableString();
        VerificationCodeExpirationDate = doc.GetNullableProperty(nameof(VerificationCodeExpirationDate))?.AsNullableLong();
        LoggingGuardLoginAttempts = doc.GetProperty(nameof(LoggingGuardLoginAttempts)).AsInt();
        LoggingGuardLockedUntil = doc.GetNullableProperty(nameof(LoggingGuardLockedUntil))?.AsNullableLong();
        Tier = doc.GetProperty(nameof(Tier));
        Filters = doc.GetProperty(nameof(Filters)).AsArrayOfString();
    }

    public static SubscriberDataModel FromDomainModel(Subscriber domainModel) => new(domainModel);

    public static SubscriberDataModel FromDocument(Document document) => new(document);

    public Type GetDomainModelType() => typeof(Subscriber);

    public Subscriber ToDomainModel()
    {
        var email = Domain.Subscribers.Email.Create(Email).Value;
        var tier = SubscribtionTier.Create(Tier).Value;

        var password = Activator.CreateInstance(
                           typeof(Password),
                           BindingFlags.Instance | BindingFlags.NonPublic,
                           null,
                           [
                               Password
                           ],
                           null) as Password ??
                       throw new DataModelConversionException(typeof(string), typeof(Password));

        var verificationCode = !string.IsNullOrWhiteSpace(VerificationCode) && VerificationCodeExpirationDate is not null ?
            Activator.CreateInstance(
                typeof(VerificationCode),
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                [VerificationCode, VerificationCodeExpirationDate],
                null) as VerificationCode ??
            throw new DataModelConversionException(
                typeof(string),
                typeof(VerificationCode)) 
            : null;

        var guard = Activator.CreateInstance(
                        typeof(LoggingGuard),
                        BindingFlags.Instance | BindingFlags.NonPublic,
                        null,
                        [
                            LoggingGuardLoginAttempts,
                            LoggingGuardLockedUntil
                        ],
                        null) as LoggingGuard ??
                    throw new DataModelConversionException(typeof(int), typeof(LoggingGuard));

        return Activator.CreateInstance(
                   typeof(Subscriber),
                   BindingFlags.Instance | BindingFlags.NonPublic,
                   null,
                   [
                       email,
                       password,
                       IsEmailVerified,
                       verificationCode,
                       guard,
                       tier,
                       Filters.ToList()
                   ],
                   null) as Subscriber ??
               throw new DataModelConversionException(typeof(Subscriber));
    }
}