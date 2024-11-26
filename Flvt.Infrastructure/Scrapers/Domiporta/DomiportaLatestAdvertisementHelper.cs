using Amazon.DynamoDBv2.Model;
using Flvt.Infrastructure.Scrapers.Shared.Helpers;
using Newtonsoft.Json;

namespace Flvt.Infrastructure.Scrapers.Domiporta;

internal sealed class DomiportaLatestAdvertisementHelper
{
    private readonly string _currentCity;
    public Dictionary<string, long> CityLastScrapedId { get; private set; }
    public List<long> CurrentlyScrapedIds { get; private set; } = [];

    public DomiportaLatestAdvertisementHelper(ScraperHelper helper, string city)
    {
        _currentCity = city;
        CityLastScrapedId = JsonConvert.DeserializeObject<Dictionary<string, long>>(helper.Value) ?? [];
    }

    public DomiportaLatestAdvertisementHelper(string city, long lastScrapedId)
    {
        _currentCity = city;
        CityLastScrapedId = new() { { city, lastScrapedId } };
    }

    public ScraperHelper ToScraperHelper()
    {
        UpdateLastScrapedId();

        return new ScraperHelper(
            nameof(DomiportaLatestAdvertisementHelper),
            JsonConvert.SerializeObject(CityLastScrapedId));
    }

    private void UpdateLastScrapedId()
    {
        if (!CurrentlyScrapedIds.Any())
        {
            return;
        }

        var isCityInDictionary = CityLastScrapedId.TryGetValue(_currentCity, out var lastScrapedId);

        if (isCityInDictionary)
        {
            CityLastScrapedId[_currentCity] = CurrentlyScrapedIds.Max() is var maxId && maxId > lastScrapedId ?
                maxId :
                lastScrapedId;
            return;
        }

        CityLastScrapedId.Add(_currentCity, CurrentlyScrapedIds.Max());
    }

    public long LastScrapedIdIn(string filterCity)
    {
        _ = CityLastScrapedId.TryGetValue(_currentCity, out var lastScrapedId);

        return lastScrapedId;
    }
}