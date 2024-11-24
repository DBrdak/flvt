using System.Security.AccessControl;
using System.Web;
using Flvt.Domain.Extensions;
using Flvt.Domain.Primitives.Advertisements;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure.Scrapers.Shared.Helpers;
using Flvt.Infrastructure.Scrapers.Shared.Parsers;
using HtmlAgilityPack;

namespace Flvt.Infrastructure.Scrapers.Domiporta;

internal sealed class DomiportaParser : AdvertisementParser
{
    protected override string GetAdvertisementNodeSelector() => "//a[@class='sneakpeak__picture_container']";
    protected string GetAdvertisementsGridNodeSelector() => "//ul[@class='grid']";

    protected override string GetContentNodeSelector() => 
        throw new InvalidOperationException("Domiporta doesn't use content selector");

    private string GetPhotoNodeSelector() =>
        "//picture[@class='gallery__container-slider__item js-gallery__item--open']/img";

    private string GetFeaturesNamesSelector() =>
        "//span[@class='features__item_name']";

    private string GetFeaturesValuesSelector() =>
        "//span[contains(@class, 'features__item_value')]";

    private string GetDescriptionSelector() =>
        "//div[@class='description__panel']";

    private string GetCoordinatesSelector(string type) =>
        $"//meta[@itemprop='{type}']";

    protected override string GetBaseUrl() => "https://www.domiporta.pl";

    protected override string GetBaseQueryRelativeUrl() => "mieszkanie/wynajme";

    public override string ParseQueryUrl(ScrapingFilter filter)
    {
        var location = filter.DomiportaLocation()?.ToLower().ReplacePolishCharacters();
        var sortByCreationDateQueryParam = "SortingOrder=InsertionDate";

        return $"{GetBaseUrl()}/{GetBaseQueryRelativeUrl()}/{location}/?{sortByCreationDateQueryParam}";
    }

    public override string ParsePagedQueryUrl(string baseQueryUrl, int page)
    {
        return $"{baseQueryUrl}&PageNumber={page}";
    }

    public override List<string> ParseAdvertisementsLinks()
    {
        var advertisementsContainer = Document.DocumentNode.SelectNodes(GetAdvertisementsGridNodeSelector()).ElementAtOrDefault(0);
        var advertisements = advertisementsContainer?.SelectNodes(GetAdvertisementNodeSelector())?.ToList();

        return advertisements?
            .Select(ad => ad.GetAttributeValue("href", string.Empty))
            .Where(link => link.StartsWith("/nieruchomosci"))
            .Select(link => string.Concat(GetBaseUrl(), link))
            .ToList() ?? [];
    }

    public override ScrapedAdContent ParseContent()
    {
        var featuresNames = Document.DocumentNode
            .SelectNodes(GetFeaturesNamesSelector())?
            .Select(node => HttpUtility.HtmlDecode(node.InnerText.Trim()))
            .ToList();
        var featuresValues = Document.DocumentNode
            .SelectNodes(GetFeaturesValuesSelector())?
            .Select(node => HttpUtility.HtmlDecode(node.InnerText.Trim()))
            .ToList();

        var longitude = Document.DocumentNode
            .SelectSingleNode(GetCoordinatesSelector("longitude"))?
            .GetAttributeValue("content", string.Empty)
            .Replace(',', '.') ?? string.Empty;
        var latitude = Document.DocumentNode
            .SelectSingleNode(GetCoordinatesSelector("longitude"))?
            .GetAttributeValue("content", string.Empty)
            .Replace(',', '.') ?? string.Empty;
        var coordinates = new Coordinates(longitude, latitude);

        var features = featuresNames is null || featuresValues is null
            ? new Dictionary<string, string>() 
            : featuresNames
                .Zip(featuresValues, (name, value) => new KeyValuePair<string, string>(name, value))
                .ToDictionary(pair => pair.Key, pair => pair.Value);

        features.Add("coordinates", coordinates.ToString());

        var description = HttpUtility.HtmlDecode(
            Document.DocumentNode
                .SelectSingleNode(GetDescriptionSelector())?
                .InnerText.Trim() ??
            string.Empty);

        return new DomiportaAdContent(features, description);
    }

    public override IEnumerable<string> ParsePhotos() =>
        Document.DocumentNode
            .SelectNodes(GetPhotoNodeSelector())?
            .Select(node => node.GetAttributeValue("src", string.Empty))
            .ToList() ?? [];

    public override bool IsRateLimitExceeded(HtmlDocument htmlDocument)
    {
        return false;
    }

    public override bool IsOutdatedAdvertisement()
    {
        return false;
    }
}