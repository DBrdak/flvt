using Flvt.Domain.Advertisements;
using Flvt.Domain.Primitives;

namespace Flvt.Application.Abstractions;

public interface IProcessingOrchestrator
{
    Task<Result<IEnumerable<ProcessedAdvertisement>>> ProcessAsync(
        IEnumerable<ScrapedAdvertisement> scrapedAdvertisements);
}