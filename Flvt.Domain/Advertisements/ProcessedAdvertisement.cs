namespace Flvt.Domain.Advertisements;

public sealed class ProcessedAdvertisement
{
    public string Address { get; init; }
    public string ContactName { get; init; }
    public string ContactType { get; init; }
    public string Link { get; init; }
    public int Price { get; init; }
    public int Rooms { get; init; }
    public string Floor { get; init; }
    public int Area { get; init; }

    public ProcessedAdvertisement(
        string address,
        string contactName,
        string contactType,
        string link,
        int price,
        int rooms,
        string floor,
        int area)
    {
        Address = address;
        ContactName = contactName;
        ContactType = contactType;
        Link = link;
        Price = price;
        Rooms = rooms;
        Floor = floor;
        Area = area;
    }
}