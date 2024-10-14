using Flvt.Application.Messaging;

namespace Flvt.Application.Subscribers.Register;

public sealed record RegisterCommand(string Email, string CountryCode) : ICommand;
