using System.Net.Http.Json;
using Flvt.Domain.Primitives.Responses;
using Flvt.Infrastructure.Messanger.Emails.Models;
using Flvt.Infrastructure.Messanger.Emails.Resend.Models;
using Flvt.Infrastructure.SecretManager;
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
        _ = _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {apiToken}");

        var resendEmail = ResendEmail.FromDomainEmail(email);

        var response = await _httpClient.PostAsJsonAsync(
            sendEmailPath,
            resendEmail);

        var responseContent = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            return Result.Success();
        }

        Log.Logger.Error(
            "Problem while sending an email to {recipient}. Response: {errorMessage}.",
            email.Recipient,
            responseContent);

        return CreateResendResponseError();

    }

    private static Error CreateResendResponseError() => new(
        $"An error occured when we were trying to send an email to you. Please try again later");
}