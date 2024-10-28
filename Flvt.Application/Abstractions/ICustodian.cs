using Flvt.Domain.Photos;
using Flvt.Domain.ScrapedAdvertisements;

namespace Flvt.Application.Abstractions;

public interface ICustodian
{
    Task<IEnumerable<string>> FindOutdatedAdvertisementsAsync(IEnumerable<string> processedAdvertisementsLinks);
    Task<IEnumerable<string>> FindDuplicateAdvertisementsAsync();

    Task<IEnumerable<ScrapedAdvertisement>> FindUnsucessfullyProcessedAdvertisements(
        IEnumerable<ScrapedAdvertisement> scrapedAdvertisements);

    Task<IEnumerable<string>> FindUnusedAdvertisementPhotos(
        List<string> advertisementsLinks,
        List<string> advertisementsPhotos);
}