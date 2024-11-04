using System.Net.Http.Json;
using Flvt.Domain.Primitives.Responses;
using Flvt.Infrastructure.Messanger.Emails.Models;
using Flvt.Infrastructure.Messanger.Emails.Resend.Models;
using Flvt.Infrastructure.SecretManager;
using Newtonsoft.Json;
using Serilog;

namespace Flvt.Infrastructure.Messanger.Emails.Resend;

internal sealed class ResendClient
{
    private readonly HttpClient _httpClient = new();
    private const string sendEmailPath = "https://api.resend.com/emails";
    private const string secretName = "flvt/resend";

    public async Task<Result> SendEmailAsync(Email email)
    {
        var apiToken = await SecretAccesor.GetSecretAsync(secretName);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiToken}");

        var resendEmail = ResendEmail.FromDomainEmail(email);

        var response = await _httpClient.PostAsJsonAsync(
            sendEmailPath,
            resendEmail);

        var responseContent = await response.Content.ReadAsStringAsync();

        var jsonData = JsonConvert.SerializeObject(email);

        var result = Result.FromBool(
            response.IsSuccessStatusCode,
            CreateResendResponseError([email.Recipient], responseContent, jsonData));

        if (result.IsFailure)
        {
            Log.Logger.Error(
                "Problem while sending an email to {recipient}. Response: {errorMessage}.",
                email.Recipient,
                responseContent);
        }

        return result;
    }

    private static Error CreateResendResponseError(string[] recipient, object errorMessage, string data) => new(
        $"Problem while sending an email to {JsonConvert.SerializeObject(recipient)}");
}