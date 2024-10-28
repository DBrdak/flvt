using Flvt.Application.Abstractions;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.Subscribers;
using Flvt.Infrastructure.Messanger.Emails.Models;
using Flvt.Infrastructure.Messanger.Emails.Resend;
using Microsoft.AspNetCore.Routing.Template;
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
}