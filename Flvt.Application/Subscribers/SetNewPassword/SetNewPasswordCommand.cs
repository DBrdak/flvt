using Flvt.Application.Messaging;
using Flvt.Application.Subscribers.Models;

namespace Flvt.Application.Subscribers.SetNewPassword;

public sealed record SetNewPasswordCommand(
    string SubscriberEmail,
    string VerificationCode,
    string NewPassword) : ICommand<SubscriberModel>;
