using Flvt.Domain.Extensions;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure.Scrapers.Shared.Helpers;
using Flvt.Infrastructure.Scrapers.Shared.Parsers;
using HtmlAgilityPack;

namespace Flvt.Infrastructure.Scrapers.Otodom;

internal sealed class OtodomParser : AdvertisementParser
{
    private OtodomAdAdContent? _content;

    protected override string GetAdvertisementNodeSelector() => "//a[@data-cy='listing-item-link']";

    protected override string GetContentNodeSelector() => "//script[@id='__NEXT_DATA__']";

    protected override string GetBaseUrl() => "https://www.otodom.pl";

    protected override string GetBaseQueryRelativeUrl() => "pl/wyniki/wynajem/mieszkanie";

    public override string ParseQueryUrl(ScrapingFilter filter)
    {
        var location = filter.OtodomLocation()?.ToLower().ReplacePolishCharacters();
        var createdInLast24H = filter.OnlyNew ? "daysSinceCreated=1" : string.Empty;

        return $"{GetBaseUrl()}/{GetBaseQueryRelativeUrl()}/{location}/?{createdInLast24H}";
    }

    public override string ParsePagedQueryUrl(string baseQueryUrl, int page) => $"{baseQueryUrl}&page={page}";

    public override List<string> ParseAdvertisementsLinks()
    {
        var advertisements = Document.DocumentNode.SelectNodes(GetAdvertisementNodeSelector())?.ToList();

        return advertisements?.Select(
                ad => string.Concat(
                    GetBaseUrl(),
                    ad.GetAttributeValue(
                        "href",
                        string.Empty)))
            .ToList() ?? [];
    }

    public override ScrapedAdContent ParseContent()
    {
        var nodeJson = Document.DocumentNode.SelectSingleNode(GetContentNodeSelector()).InnerHtml;

        _content = OtodomAdAdContent.FromJson(nodeJson);

        return _content;
    }

    public override IEnumerable<string> ParsePhotos() => _content?.Images.Select(image => image.Large) ?? [];

    public override bool IsRateLimitExceeded(HtmlDocument htmlDocument) =>
        htmlDocument
            .DocumentNode
            .SelectSingleNode("//title")
            .InnerText
            .ToLower() is var title &&
        title.Contains("error: the request could not be satisfied");

    public override bool IsOutdatedAdvertisement() =>
        Document
                .DocumentNode
                .SelectSingleNode("//div[@data-cy='expired-ad-alert']")
            is not null;
}