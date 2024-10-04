using System.Runtime.CompilerServices;
using Flvt.Domain.Advertisements;
using Flvt.Domain.Extensions;
using HtmlAgilityPack;

namespace Flvt.Scraper.Morizon;

internal sealed class MorizonScraper : WebScraper
{
    private const string baseUrl = "https://www.morizon.pl/do-wynajecia/mieszkania";
    private readonly HashSet<string> _advertisementsLinks = [];

    public MorizonScraper(Filter filter) : base(filter, baseUrl)
    {
        BuildQueryUrl();
    }

    public override async Task<IEnumerable<ScrapedAdvertisement>> ScrapeAsync()
    {
        await ScrapeAdvertisementsLinksAsync();
        
        return null;
    }

    private async Task ScrapeAdvertisementsLinksAsync()
    {
        var page = 1;
        bool isValidPage;

        do
        {
            var pageUrl = $"{QueryUrl}&page={page}";
            var client = new HttpClient();
            var g = await client.GetAsync(pageUrl);
            var c = await g.Content.ReadAsStringAsync();
            var htmlDoc = new HtmlDocument();

            htmlDoc.LoadHtml(c);

            var advertisements = htmlDoc.DocumentNode.SelectNodes("//a[contains(@class, 'q-Oe37')]").ToList();

            advertisements.ForEach(ad => _advertisementsLinks.Add(ad.GetAttributeValue("href", string.Empty)));

            isValidPage = page == 1 || htmlDoc.DocumentNode.SelectSingleNode("//a[contains(@class, 'QnjRFa')]") is not null;
            page++;
        }
        while (isValidPage);
    }

    protected override void BuildQueryUrl()
    {
        var location = Filter.Location.ToLower().ReplacePolishCharacters();

        var minPrice = Filter.MinPrice is null ? string.Empty : $"ps[price_from]={Filter.MinPrice}&";
        var maxPrice = Filter.MaxPrice is null ? string.Empty : $"ps[price_to]={Filter.MaxPrice}&";

        var minArea = Filter.MinArea is null ? string.Empty : $"ps[living_area_from]={Filter.MinArea}&";
        var maxArea = Filter.MaxArea is null ? string.Empty : $"ps[living_area_to]={Filter.MaxArea}&";

        var minRooms = Filter.MinRooms is null ? string.Empty : $"ps[number_of_rooms_from]={Filter.MinRooms}&";
        var maxRooms = Filter.MaxRooms is null ? string.Empty : $"ps[number_of_rooms_to]={Filter.MaxRooms}&";

        QueryUrl = $"{BaseUrl}/{location}/?{minPrice}{maxPrice}{minArea}{maxArea}{minRooms}{maxRooms}";
    }
}