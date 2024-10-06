using Flvt.Application.Abstractions;
using Flvt.Domain.Advertisements;
using Flvt.Domain.Extensions;
using Flvt.Domain.Subscribers;
using Newtonsoft.Json;
using Serilog;

namespace Flvt.Infrastructure.Scrapers.Morizon;

internal sealed class MorizonScraper : WebScraper
{
    private const string baseUrl = "https://www.morizon.pl";
    private const string baseSearchUrl = "https://www.morizon.pl/do-wynajecia/mieszkania";
    private const string advertisementNodeSelector = "//a[contains(@class, 'q-Oe37')]";
    private const string nextPageNodeSelector = "//a[contains(@class, 'QnjRFa')]";
    private const string descriptionNodeSelector = "//div[contains(@class, '_0A-7u8')]";
    private const string priceNodeSelector = "//span[contains(@class, 'LphL0t')]";
    private const string contactTypeNodeSelector = "//div[contains(@class, 'lf4Mw8')]";
    private const string locationNodeSelector = "//h2[contains(@class, 'y1mnyH')]";
    private const string floorRoomsAreaNodeSelector = "//div[contains(@class, 'Ca6gX5')]";
    private const string addedAtNodeSelector = "//div[contains(@class, 'vZJg9t') and .//span[text()='Data dodania']]//div[@data-cy='itemValue']";
    private const string updatedAtNodeSelector = "//div[contains(@class, 'vZJg9t') and .//span[text()='Aktualizacja']]//div[@data-cy='itemValue']";
    private const char roomsFloorAreaSeparator = '•';
    private const int roomsIndex = 1;
    private const int areaIndex = 2;
    private const int floorIndex = 3;
    private readonly HashSet<string> _advertisementsLinks = [];
    private readonly List<ScrapedAdvertisement> _advertisements = [];

    public MorizonScraper(Filter filter) : base(filter)
    {
        BuildQueryUrl();
    }

    public override async Task<IEnumerable<ScrapedAdvertisement>> ScrapeAsync()
    {
        try
        {
            await ScrapeAdvertisementsLinksAsync();
        }
        catch (Exception e)
        {
            Log.Logger.Error(
                $"Exception occured when trying to scrape advertisement links: {JsonConvert.SerializeObject(e)}");
        }

        try
        {
            await ScrapeAdvertisementsContentAsync();
        }
        catch (Exception e)
        {
            Log.Logger.Error(
                $"Exception occured when trying to scrape advertisement content: {JsonConvert.SerializeObject(e)}");
        }

        return _advertisements;
    }

    private async Task ScrapeAdvertisementsContentAsync()
    {
        var tasks = new List<Task>();

        foreach (var advertisementLink in _advertisementsLinks)
        {
            var task = ScrapeAdvertisementContentAsync(advertisementLink);
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
    }

    private async Task ScrapeAdvertisementContentAsync(string advertisementLink)
    {
        var htmlDoc = await Web.LoadFromWebAsync(advertisementLink);

        var location = htmlDoc.DocumentNode.SelectSingleNode(locationNodeSelector)?.InnerText;
        var description = htmlDoc.DocumentNode.SelectSingleNode(descriptionNodeSelector)?.InnerText;
        var contactType = htmlDoc.DocumentNode.SelectSingleNode(contactTypeNodeSelector)?.InnerText ?? "agencja nieruchomości";
        var price = htmlDoc.DocumentNode.SelectSingleNode(priceNodeSelector)?.InnerText;
        var priceAmount = string.Join("", price?.Where(char.IsDigit) ?? "");
        var priceCurrency = string.Join("", price?.Where(char.IsLetter) ?? "");
        var roomsFloorArea = htmlDoc.DocumentNode.SelectSingleNode(floorRoomsAreaNodeSelector)?.InnerText;
        var roomsCount = roomsFloorArea?.Split(roomsFloorAreaSeparator).ElementAtOrDefault(roomsIndex)?.Trim().Split(" ").ElementAtOrDefault(0);
        var roomsUnit = roomsFloorArea?.Split(roomsFloorAreaSeparator).ElementAtOrDefault(roomsIndex)?.Trim().Split(" ").ElementAtOrDefault(1);
        var areaValue = roomsFloorArea?.Split(roomsFloorAreaSeparator).ElementAtOrDefault(areaIndex)?.Trim().Split(" ").ElementAtOrDefault(0);
        var areaUnit = roomsFloorArea?.Split(roomsFloorAreaSeparator).ElementAtOrDefault(areaIndex)?.Trim().Split(" ").ElementAtOrDefault(1);
        var floor = roomsFloorArea?.Split(roomsFloorAreaSeparator).ElementAtOrDefault(floorIndex)?.Trim();
        var addedAt = htmlDoc.DocumentNode.SelectSingleNode(addedAtNodeSelector)?.InnerText;
        var updatedAt = htmlDoc.DocumentNode.SelectSingleNode(updatedAtNodeSelector)?.InnerText;

        var createResult = ScrapedAdvertisement.CreateFromScrapedContent(
            advertisementLink,
            location,
            description,
            contactType,
            priceAmount,
            priceCurrency,
            roomsCount,
            roomsUnit,
            areaValue, 
            areaUnit,
            floor,
            addedAt,
            updatedAt);

        if (createResult.IsSuccess)
        {
            _advertisements.Add(createResult.Value);
            return;
        }

        Log.Error($"Failed to create ScrapedAdvertisement, error: {createResult.Error}. Advertisement link: {advertisementLink}");
    }

    private async Task ScrapeAdvertisementsLinksAsync()
    {
        var page = 1;
        bool isValidPage;

        do
        {
            var pageUrl = $"{QueryUrl}&page={page}";
            var htmlDoc = await Web.LoadFromWebAsync(pageUrl);

            var advertisements = htmlDoc.DocumentNode.SelectNodes(advertisementNodeSelector).ToList();

            advertisements.ForEach(
                ad => _advertisementsLinks.Add(
                    string.Concat(
                        baseUrl,
                        ad.GetAttributeValue(
                            "href",
                            string.Empty))));

            isValidPage = page == 1 || htmlDoc.DocumentNode.SelectSingleNode(nextPageNodeSelector) is not null;
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

        QueryUrl = $"{baseSearchUrl}/{location}/?{minPrice}{maxPrice}{minArea}{maxArea}{minRooms}{maxRooms}";
    }
}