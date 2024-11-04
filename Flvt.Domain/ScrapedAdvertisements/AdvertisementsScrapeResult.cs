using Flvt.Domain.Photos;

namespace Flvt.Domain.ScrapedAdvertisements;

public sealed record AdvertisementsScrapeResult
{
    public IReadOnlyCollection<ScrapedAdvertisement> ScrapedAdvertisements { get; init; }
    public IReadOnlyCollection<AdvertisementPhotos> Photos { get; init; }

    public AdvertisementsScrapeResult(
        IEnumerable<ScrapedAdvertisement> scrapedAdvertisements,
        IEnumerable<AdvertisementPhotos> photos)
    {
        ScrapedAdvertisements = scrapedAdvertisements.ToList();
        Photos = photos.ToList();
    }
}