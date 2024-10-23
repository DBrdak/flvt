using Flvt.Application.Messaging;

namespace Flvt.Application.Advertisements.MarkAsSeen;

public sealed record MarkAsSeenCommand(string SubscriberEmail, string AdvertisementLink) : ICommand;
