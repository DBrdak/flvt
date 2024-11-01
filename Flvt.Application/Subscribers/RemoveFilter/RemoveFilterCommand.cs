using Flvt.Application.Messaging;
using Flvt.Application.Subscribers.Models;

namespace Flvt.Application.Subscribers.RemoveFilter;

public sealed record RemoveFilterCommand(string SubscriberEmail, string FilterId) : ICommand<SubscriberModel>;
