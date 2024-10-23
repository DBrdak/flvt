using Flvt.Domain.Filters;
using Flvt.Domain.Subscribers;

namespace Flvt.Application.Subscribers.Models;

public sealed record FilterModel
{
    public string Id { get; init; }
    public string Name { get; private set; }
    public string Location { get; private set; }
    public decimal? MinPrice { get; private set; }
    public decimal? MaxPrice { get; private set; }
    public int? MinRooms { get; private set; }
    public int? MaxRooms { get; private set; }
    public decimal? MinArea { get; private set; }
    public decimal? MaxArea { get; private set; }
    public Preferences? Preferences { get; init; }
    public string Tier { get; init; }
    public bool OnlyLast24H { get; init; }
    //TODO consider below
    public IReadOnlyList<string> FoundAdvertisements { get; init; }
    public IReadOnlyList<string> RecentlyFoundAdvertisements { get; init; }
    public IReadOnlyList<string> SeenAdvertisements { get; init; }
}