using Flvt.Application.Messaging;
using Flvt.Application.Subscribers.Models;

namespace Flvt.Application.Subscribers.GetSubscriber;

public sealed record GetSubscriberQuery(string SubscriberEmail) : IQuery<SubscriberModel>;
