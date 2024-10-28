using Flvt.Domain.ProcessedAdvertisements;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure.Data.Repositories;
using Serilog;

namespace Flvt.Infrastructure.Custodians.Assistants;

internal sealed class DataCustodialAssistant
{
    private readonly IProcessedAdvertisementRepository _processedAdvertisementRepository;
    private readonly BatchRepository _batchRepository;

    public DataCustodialAssistant(
        IProcessedAdvertisementRepository processedAdvertisementRepository,
        BatchRepository batchRepository)
    {
        _processedAdvertisementRepository = processedAdvertisementRepository;
        _batchRepository = batchRepository;
    }

    public async Task<List<string>> SearchForDuplicatedAdvertisementsAsync()
    {
        var advertisementsGetResult = await _processedAdvertisementRepository.GetAllAsync();

        if (advertisementsGetResult.IsFailure)
        {
            Log.Logger.ForContext<DataCustodialAssistant>().Error(
                "Failed to retireve Processed Advertisements, error: {error}",
                advertisementsGetResult.Error);
            return [];
        }

        var advertisements = advertisementsGetResult.Value;

        return advertisements
            .GroupBy(ad => ad.Dedupe)
            .Where(group => group.Count() > 1)
            .SelectMany(group => group)
            .DistinctBy(ad => ad.Dedupe)
            .Select(ad => ad.Link)
            .ToList();
    }

    public async Task<List<ScrapedAdvertisement>> GetAdvertisementsInProcessAndNotInBatchAsync(
        IEnumerable<ScrapedAdvertisement> scrapedAdvertisements)
    {
        var batchesGetResult = await _batchRepository.GetAllAsync();

        if (batchesGetResult.IsFailure)
        {
            Log.Logger.ForContext<DataCustodialAssistant>().Error(
                "Failed to retireve Batches, error: {error}",
                batchesGetResult.Error);

            return [];
        }

        var batches = batchesGetResult.Value.ToList();

        return !batches.Any() ?
            scrapedAdvertisements.ToList() :
            scrapedAdvertisements
                .Where(sa => !batches.Any(b => b.ProcessingAdvertisementsLinks.Contains(sa.Link)))
                .ToList();
    }

    public async Task<List<string>> GetUnusedPhotos(
        IEnumerable<string> advertisementsLinks,
        IEnumerable<string> advertisementsPhotosLinks) =>
        advertisementsPhotosLinks
            .Except(advertisementsLinks)
            .ToList();
}