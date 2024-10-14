using Flvt.Domain.ScrapedAdvertisements;

namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Responses;

internal sealed record AdvertisementsFile(
    string FileId,
    List<ScrapedAdvertisement> AdvertisementsInFileAsync);