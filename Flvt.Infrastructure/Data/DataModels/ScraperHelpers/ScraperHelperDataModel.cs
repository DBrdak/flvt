using Amazon.DynamoDBv2.DocumentModel;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure.Data.DataModels.ScrapedAdvertisements;
using Flvt.Infrastructure.Data.Extensions;
using Flvt.Infrastructure.Scrapers.Shared.Helpers;

namespace Flvt.Infrastructure.Data.DataModels.ScraperHelpers;

internal sealed class ScraperHelperDataModel : IDataModel<ScraperHelper>
{
    public string Name { get; init; }
    public string Value { get; init; }

    public ScraperHelperDataModel(ScraperHelper original)
    {
        Name = original.Name;
        Value = original.Value;
    }

    public ScraperHelperDataModel(Document doc)
    {
        Name = doc.GetProperty(nameof(Name));
        Value = doc.GetProperty(nameof(Value));
    }

    public static ScraperHelperDataModel FromDomainModel(ScraperHelper domainModel) => new(domainModel);

    public static ScraperHelperDataModel FromDocument(Document document) => new(document);


    public Type GetDomainModelType() => typeof(ScraperHelper);

    public ScraperHelper ToDomainModel() =>
        new(Name, Value);
}