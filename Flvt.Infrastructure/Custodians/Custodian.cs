using Flvt.Application.Abstractions;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure.AWS.Contants;
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
        var linksChunks = processedAdvertisementsLinks.Chunk(AWSLimits.FileDescriptorLimit / 2);
        List<string> outdatedAdvertisements = [];

        foreach (var chunk in linksChunks)
        {
            var tasks = chunk.Select(_scrapingAssistant.ChekIfAdvertisementIsOutdatedAsync);
            var outdatedAdvertisementsInChunk = await Task.WhenAll(tasks);

            outdatedAdvertisements.AddRange(
                outdatedAdvertisementsInChunk
                    .Where(ad => ad is not null)
                    .Select(ad => ad!));
        }

        Log.Logger.Information(
            "Found {outdatedCount} outdated advertisements",
            outdatedAdvertisements.Count);

        return outdatedAdvertisements;
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