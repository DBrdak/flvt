using Flvt.Application.Messaging;
using Flvt.Application.Subscribers.Models;

namespace Flvt.Application.Subscribers.Register;

public sealed record RegisterCommand(string Email, string Password) : ICommand<SubscriberModel>;
