using Amazon.DynamoDBv2.DocumentModel;
using Flvt.Domain.ScrapedAdvertisements;

namespace Flvt.Infrastructure.Data.DataModels.ScrapedAdvertisements;

internal sealed class ScrapedAdvertisementDataModel : IDataModel<ScrapedAdvertisement>
{
    public string Link { get; init; }
    public string AdContent { get; init; }
    public IEnumerable<string> Photos { get; init; }
    public bool IsProcessed { get; init; }
    public long? ProcessingStartedAt { get; init; }

    public ScrapedAdvertisementDataModel(ScrapedAdvertisement original)
    {
        Link = original.Link;
        AdContent = original.AdContent;
        Photos = original.Photos;
        IsProcessed = original.IsProcessed;
        ProcessingStartedAt = original.ProcessingStartedAt;
    }

    public ScrapedAdvertisementDataModel(Document doc)
    {
        Link = doc[nameof(Link)];
        AdContent = doc[nameof(AdContent)];
        Photos = doc[nameof(Photos)].AsArrayOfString();
        IsProcessed = doc[nameof(Photos)].AsBoolean();
        ProcessingStartedAt = doc[nameof(ProcessingStartedAt)].AsLong();
    }

    public static ScrapedAdvertisementDataModel FromDomainModel(ScrapedAdvertisement domainModel) => new(domainModel);

    public static ScrapedAdvertisementDataModel FromDocument(Document document) => new(document);


    public Type GetDomainModelType() => typeof(ScrapedAdvertisement);

    public ScrapedAdvertisement ToDomainModel() =>
        new(
            Link,
            AdContent,
            Photos,
            ProcessingStartedAt,
            IsProcessed);
}