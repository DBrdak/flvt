using Flvt.Application.Messaging;

namespace Flvt.Application.Advertisements.ScrapeAdvertisementsLinks;

public sealed record ScrapeAdvertisementsLinksCommand(string Service) : ICommand;
