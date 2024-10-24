using Flvt.Application.Messaging;

namespace Flvt.Application.Advertisements.MarkAsSeen;

public sealed record MarkAsSeenCommand(string FilterId, string AdvertisementLink) : ICommand;
