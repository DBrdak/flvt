using Flvt.Domain.ScrapedAdvertisements;

namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Responses;

internal sealed record AdvertisementsBatch(
    string BatchId,
    List<ScrapedAdvertisement> AdvertisementsInBatchAsync);