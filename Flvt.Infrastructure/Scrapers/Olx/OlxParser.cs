using Flvt.Domain.Extensions;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure.Scrapers.Shared.Helpers;
using Flvt.Infrastructure.Scrapers.Shared.Parsers;
using HtmlAgilityPack;

namespace Flvt.Infrastructure.Scrapers.Olx;

internal sealed class OlxParser : AdvertisementParser
{
    private OlxAdContent? _adContent;

    private const string attributesSelector = "//div[@id='baxter-above-parameters']/following-sibling::*[1]";
    private const string locationSelector =
        "//div[contains(@class, 'qa-static-ad-map-container')]";

    protected override string GetAdvertisementNodeSelector() =>
        "//div[@data-cy='ad-card-title']/a";

    protected override string GetContentNodeSelector() => "//script[@type='application/ld+json']";

    protected override string GetBaseUrl() => "https://www.olx.pl";

    protected override string GetBaseQueryRelativeUrl() => "nieruchomosci/mieszkania/wynajem";

    public override string ParseQueryUrl(ScrapingFilter filter)
    {
        var location = filter.City.ToLower().ReplacePolishCharacters();

        return $"{GetBaseUrl()}/{GetBaseQueryRelativeUrl()}/{location}/";
    }

    public override string ParsePagedQueryUrl(string baseQueryUrl, int page) => $"{baseQueryUrl}&page={page}";

    public override List<string> ParseAdvertisementsLinks()
    {
        var advertisements = Document.DocumentNode.SelectNodes(GetAdvertisementNodeSelector()).ToList();
        var relativeLinks = advertisements.Select(
                ad => ad.GetAttributeValue(
                    "href",
                    string.Empty))
            .Where(link => !link.ToLower().Contains("otodom"))
            .ToList();

        return relativeLinks.Distinct().Select(link => string.Concat(GetBaseUrl(), link)).ToList();
    }

    public override ScrapedAdContent ParseContent()
    {
        var infoJson = Document.DocumentNode.SelectSingleNode(GetContentNodeSelector()).InnerText;
        var info = OlxInfo.FromJson(infoJson);
        var attributes = Document.DocumentNode.SelectSingleNode(attributesSelector).InnerText;
        var a = Document.DocumentNode.SelectSingleNode(locationSelector);
        var location = Document.DocumentNode.SelectSingleNode(locationSelector).GetAttributeValue("src", null);

        _adContent = new OlxAdContent(info, attributes, location);

        return _adContent;
    }

    public override IEnumerable<string> ParsePhotos() => _adContent?.Info.Image ?? [];

    public override bool IsRateLimitExceeded(HtmlDocument htmlDocument)
    {
        return false;
    }

    public override bool IsOutdatedAdvertisement()
    {
        return false;
    }
}

