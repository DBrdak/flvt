namespace Flvt.Domain.Photos;

public sealed class AdvertisementPhotos
{
    public string AdvertisementLink { get; init; }
    public IEnumerable<string> Links { get; init; }

    public AdvertisementPhotos(string advertisementLink, IEnumerable<string> links)
    {
        AdvertisementLink = advertisementLink;
        Links = links;
    }
}