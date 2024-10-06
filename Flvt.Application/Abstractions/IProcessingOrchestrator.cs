using Flvt.Domain.Advertisements;

namespace Flvt.Application.Abstractions;

public interface IProcessingOrchestrator
{
    Task ProcessAsync(IEnumerable<ScrapedAdvertisement> scrapedAdvertisements);
}