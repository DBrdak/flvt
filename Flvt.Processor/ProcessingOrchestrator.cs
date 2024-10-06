using Flvt.Application.Abstractions;
using Flvt.Domain.Advertisements;

namespace Flvt.Processor;

internal sealed class ProcessingOrchestrator : IProcessingOrchestrator
{
    public async Task ProcessAsync(IEnumerable<ScrapedAdvertisement> scrapedAdvertisements)
    {
    }
}