using Flvt.Application.Messaging;
using Flvt.Application.Subscribers.Models;

namespace Flvt.Application.Subscribers.VerifyEmail;

public sealed record VerifyEmailCommand(string Email, string VerificationCode) : ICommand<SubscriberModel>;
