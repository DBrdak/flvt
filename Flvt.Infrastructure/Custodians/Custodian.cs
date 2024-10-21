using Flvt.Application.Abstractions;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure.Custodians.Assistants;
using Serilog;

namespace Flvt.Infrastructure.Custodians;

internal class Custodian : ICustodian
{
    private readonly DataCustodialAssistant _dataAssistant;
    private readonly ScrapingCustodialAssistant _scrapingAssistant;

    public Custodian(
        ScrapingCustodialAssistant scrapingAssistant, 
        DataCustodialAssistant dataAssistant)
    {
        _scrapingAssistant = scrapingAssistant;
        _dataAssistant = dataAssistant;
    }

    public async Task<IEnumerable<string>> FindOutdatedAdvertisementsAsync(
        IEnumerable<string> processedAdvertisementsLinks)
    {
        var checkingTasks = processedAdvertisementsLinks
            .Select(_scrapingAssistant.ChekIfAdvertisementIsOutdatedAsync);

        var outdatedAdvertisements = await Task.WhenAll(checkingTasks);

        Log.Logger.Information(
            "Found {outdatedCount} outdated advertisements",
            outdatedAdvertisements.Length);

        return outdatedAdvertisements.Where(ad => ad is not null).Select(ad => ad!);
    }

    public async Task<IEnumerable<string>> FindDuplicateAdvertisementsAsync()
    {
        var duplicates = await _dataAssistant.SearchForDuplicatedAdvertisementsAsync();

        Log.Logger.Information("Found {duplicateCount} duplicate advertisements", duplicates.Count);

        return duplicates;
    }

    public async Task<IEnumerable<ScrapedAdvertisement>> FindUnsucessfullyProcessedAdvertisements(IEnumerable<ScrapedAdvertisement> scrapedAdvertisements)
    {
        var unsucessfullyProcessedAds =
            await _dataAssistant.GetAdvertisementsInProcessAndNotInBatchAsync(scrapedAdvertisements);

        Log.Logger.Information(
            "Found {unsucessfullyProcessedAds} unsucessfully processed advertisements",
            unsucessfullyProcessedAds.Count);

        unsucessfullyProcessedAds.ForEach(ad => ad.ProcessFailed());

        return unsucessfullyProcessedAds;
    }
}