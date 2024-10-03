using Flvt.Domain.Advertisements;
using Flvt.Domain.Extensions;
using Flvt.Scraper.Extensions;
using OpenQA.Selenium;

namespace Flvt.Scraper.Morizon;

internal sealed class MorizonScraper : WebScraper
{
    private const string baseUrl = "https://www.morizon.pl/do-wynajecia/mieszkania";

    public MorizonScraper(Filter filter) : base(filter, baseUrl)
    {
        BuildQueryUrl();
    }

    public override async Task<IEnumerable<ScrapedAdvertisement>> ScrapeAsync()
    {
        await Browser.Navigate().GoToUrlAsync(QueryUrl);

        var advertisements = Browser.Wait().Until(d => d.FindElements(By.ClassName("q-Oe37")));
        var advertisementsLinks = advertisements.Select(ad => ad.GetAttribute("href"));

        return null;
    }

    protected override void BuildQueryUrl()
    {
        var location = Filter.Location.ToLower().ReplacePolishCharacters();
        var maxPrice = Filter.MaxPrice is null ?
                string.Empty : $"do-{Filter.MaxPrice}/";

        var minArea = Filter.MaxArea is null ? string.Empty : $"ps[living_area_from]={Filter.MinArea}";
        var maxArea = Filter.MaxArea is null ? string.Empty : $"ps[living_area_to]={Filter.MaxArea}";

        var minRooms = Filter.MinRooms;
        var maxRooms = Filter.MaxRooms;
        var rooms = string.Empty;

        for (var i = 0; i <= minRooms - maxRooms; i++)
        {
            rooms += $"&ps[number_of_rooms][{i}]={minRooms + i}";
        }

        QueryUrl = $"{BaseUrl}/{maxPrice}{location}/?{minArea}{maxArea}{rooms}";
    }
}