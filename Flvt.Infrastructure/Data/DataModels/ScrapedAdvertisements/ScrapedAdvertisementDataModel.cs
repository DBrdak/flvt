using Amazon.DynamoDBv2.DocumentModel;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure.Data.Extensions;

namespace Flvt.Infrastructure.Data.DataModels.ScrapedAdvertisements;

internal sealed class ScrapedAdvertisementDataModel : IDataModel<ScrapedAdvertisement>
{
    public string Link { get; init; }
    public string AdContent { get; init; }
    public bool IsProcessed { get; init; }
    public long? ProcessingStartedAt { get; init; }

    public ScrapedAdvertisementDataModel(ScrapedAdvertisement original)
    {
        Link = original.Link;
        AdContent = original.AdContent;
        IsProcessed = original.IsProcessed;
        ProcessingStartedAt = original.ProcessingStartedAt;
    }

    public ScrapedAdvertisementDataModel(Document doc)
    {
        Link = doc.GetProperty(nameof(Link));
        AdContent = doc.GetProperty(nameof(AdContent));
        IsProcessed = doc.GetProperty(nameof(IsProcessed)).AsBoolean();
        ProcessingStartedAt = doc.GetNullableProperty(nameof(ProcessingStartedAt))?.AsNullableLong();
    }

    public static ScrapedAdvertisementDataModel FromDomainModel(ScrapedAdvertisement domainModel) => new(domainModel);

    public static ScrapedAdvertisementDataModel FromDocument(Document document) => new(document);


    public Type GetDomainModelType() => typeof(ScrapedAdvertisement);

    public ScrapedAdvertisement ToDomainModel() =>
        new(
            Link,
            AdContent,
            ProcessingStartedAt,
            IsProcessed);
}