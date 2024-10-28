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
        "Verify Flvt",
        TemplateBuilder.GenerateEmailVerificationMessage(code));
}