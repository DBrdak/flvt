using Flvt.Application.Messaging;

namespace Flvt.Application.Subscribers.RequestNewPassword;

public sealed record RequestNewPasswordCommand(string SubscriberEmail) : ICommand;
