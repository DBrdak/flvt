using Flvt.Application.Messaging;

namespace Flvt.Application.Advertisements.Follow;

public sealed record FollowCommand(string FilterId, string AdvertisementLink) : ICommand;
