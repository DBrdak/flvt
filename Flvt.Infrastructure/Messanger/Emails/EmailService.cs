using Flvt.Application.Abstractions;
using Flvt.Domain.Filters;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.Subscribers;
using Flvt.Infrastructure.Messanger.Emails.Resend;
using Email = Flvt.Infrastructure.Messanger.Emails.Models.Email;

namespace Flvt.Infrastructure.Messanger.Emails;

internal sealed class EmailService : IEmailService
{
    private readonly ResendClient _resendClient;

    public EmailService(ResendClient resendClient)
    {
        _resendClient = resendClient;
    }

    public async Task<Result> SendVerificationEmailAsync(Subscriber subscriber)
    {
        var verificationCode = subscriber.VerificationCode?.Code;
        var emailAddress = subscriber.Email.Value;

        var email = Email.CreateVerificationEmail(emailAddress, verificationCode ?? string.Empty);

        return await _resendClient.SendEmailAsync(email);
    }

    public async Task<Result> SendResetPasswordEmailAsync(Subscriber subscriber)
    {
        var verificationCode = subscriber.VerificationCode?.Code;
        var emailAddress = subscriber.Email.Value;

        var email = Email.CreateResetPasswordEmail(emailAddress, verificationCode ?? string.Empty);

        return await _resendClient.SendEmailAsync(email);
    }

    public async Task<Result> SendFilterLaunchNotificationAsync(Subscriber subscriber, Filter filter)
    {
        var emailAddress = subscriber.Email.Value;

        var email = Email.CreateFilterLaunchNotification(
            emailAddress,
            filter.Name.Value,
            filter.Id,
            filter.RecentlyFoundAdvertisements.Count);

        return await _resendClient.SendEmailAsync(email);
    }
}