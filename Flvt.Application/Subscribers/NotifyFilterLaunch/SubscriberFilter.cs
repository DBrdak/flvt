using Flvt.Domain.Filters;
using Flvt.Domain.Subscribers;

namespace Flvt.Application.Subscribers.NotifyFilterLaunch;

internal sealed record SubscriberFilter(Subscriber Subscriber, Filter Filter);