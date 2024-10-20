using Flvt.Domain.ProcessedAdvertisements;
using Serilog;

namespace Flvt.Infrastructure.Custodians.Assistants;

internal sealed class DataCustodialAssistant
{
    private readonly IProcessedAdvertisementRepository _processedAdvertisementRepository;

    public DataCustodialAssistant(IProcessedAdvertisementRepository processedAdvertisementRepository)
    {
        _processedAdvertisementRepository = processedAdvertisementRepository;
    }

    public async Task<List<string>> SearchForDuplicatedAdvertisementsAsync()
    {
        var advertisementsGetResult = await _processedAdvertisementRepository.GetAllAsync();

        if (advertisementsGetResult.IsFailure)
        {
            Log.Logger.Error(
                "Failed to retireve Processed Advertisements, error: {error}",
                advertisementsGetResult.Error);
            return [];
        }

        var advertisements = advertisementsGetResult.Value;

        return advertisements
            .GroupBy(ad => ad.Dedupe)
            .Where(group => group.Count() > 1)
            .SelectMany(group => group)
            .Select(ad => ad.Link)
            .ToList();
    }
}