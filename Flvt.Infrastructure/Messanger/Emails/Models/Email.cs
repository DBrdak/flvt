using Flvt.Domain.Filters;

namespace Flvt.Infrastructure.Messanger.Emails.Models;

internal sealed record Email
{
    public string Recipient { get; init; }
    public string Subject { get; init; }
    public string HtmlBody { get; init; }

    private Email(string Recipient, string Subject, string HtmlBody)
    {
        this.Recipient = Recipient;
        this.Subject = Subject;
        this.HtmlBody = HtmlBody;
    }

    public static Email CreateVerificationEmail(string recipient, string code) => new(
        recipient,
        "Verify",
        TemplateBuilder.GenerateEmailVerificationMessage(code));

    public static Email CreateResetPasswordEmail(string recipient, string code) => new(
        recipient,
        "Reset Your Password",
        TemplateBuilder.GenerateResetPasswordMessage(code));

    public static Email CreateFilterLaunchNotification(string recipient, string filterName, string filterId, int count) => new(
        recipient,
        $"New Advertisements For {filterName}",
        TemplateBuilder.GenerateFilterLaunchNotificationMessage(filterName, filterId, count));
}