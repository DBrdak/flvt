using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ProcessedAdvertisements;
using Flvt.Domain.ScrapedAdvertisements;

namespace Flvt.Application.Abstractions;

public interface IProcessingOrchestrator
{
    Task<Result<IEnumerable<ProcessedAdvertisement>>> ProcessAsync(
        IEnumerable<ScrapedAdvertisement> scrapedAdvertisements);
}