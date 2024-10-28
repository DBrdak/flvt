using Flvt.Application.Messaging;
using Flvt.Application.Subscribers.Models;

namespace Flvt.Application.Subscribers.Login;

public sealed record LoginCommand(string Email, string Password) : ICommand<SubscriberModel>;
