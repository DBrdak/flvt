using Flvt.Application.Messaging;

namespace Flvt.Application.Subscribers.Register;

public sealed record RegisterCommand(string Email, string Password, string CountryCode) : ICommand;
