namespace Flvt.Domain.AdvertisementLinks;

public sealed class AdvertisementLink
{
    public string Link { get; init; }

    public AdvertisementLink(string link)
    {
        Link = link;
    }
}