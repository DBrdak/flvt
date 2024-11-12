using Flvt.Application.Messaging;

namespace Flvt.Application.Advertisements.ScrapeAdvertisements;

public sealed record ScrapeAdvertisementsCommand(IEnumerable<string> Links) : ICommand;
