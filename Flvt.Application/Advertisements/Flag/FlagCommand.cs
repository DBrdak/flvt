using Flvt.Application.Messaging;

namespace Flvt.Application.Advertisements.Flag;

public sealed record FlagCommand(string AdvertisementLink) : ICommand;
