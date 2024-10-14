using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ProcessedAdvertisements;
using Flvt.Domain.ScrapedAdvertisements;

namespace Flvt.Application.Abstractions;

public interface IProcessingOrchestrator
{
    Task<Result<List<ProcessedAdvertisement>>> ProcessAsync(
        IEnumerable<ScrapedAdvertisement> scrapedAdvertisements);

    Task<Dictionary<string, List<ScrapedAdvertisement>>> StartProcessingAsync(
        IEnumerable<ScrapedAdvertisement> scrapedAdvertisements);

    Task<List<ProcessedAdvertisement>> RetrieveProcessedAdvertisementsAsync();
}