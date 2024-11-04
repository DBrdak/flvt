using Flvt.Application.Messaging;

namespace Flvt.Application.Subscribers.ResendEmail;

public sealed record ResendEmailCommand(string Email, string Purpose) : ICommand;
