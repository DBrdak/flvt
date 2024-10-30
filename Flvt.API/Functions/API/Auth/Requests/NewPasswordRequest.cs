namespace Flvt.API.Functions.API.Auth.Requests;

internal sealed record NewPasswordRequest(string VerificationCode, string NewPassword);