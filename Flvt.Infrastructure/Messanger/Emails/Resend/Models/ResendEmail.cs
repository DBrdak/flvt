using Newtonsoft.Json;
using Email = Flvt.Infrastructure.Messanger.Emails.Models.Email;

namespace Flvt.Infrastructure.Messanger.Emails.Resend.Models;

internal class ResendEmail
{
    private const string baseSender = "Flvt <notify@onyxapp.tech>";

    [JsonProperty("from")]
    public string From { get; init; }
    [JsonProperty("to")]
    public string[] To { get; init; }
    [JsonProperty("subject")]
    public string Subject { get; init; }
    [JsonProperty("html")]
    public string Html { get; init; }

    private ResendEmail(string[] to, string subject, string html)
    {
        From = baseSender;
        To = to;
        Subject = subject;
        Html = html;
    }

    [JsonConstructor]
    private ResendEmail(string from, string[] to, string subject, string html)
    {
        From = from;
        To = to;
        Subject = subject;
        Html = html;
    }

    public static ResendEmail FromDomainEmail(Email email)
    {
        return new(
            [email.Recipient],
            email.Subject,
            email.HtmlBody);
    }
}