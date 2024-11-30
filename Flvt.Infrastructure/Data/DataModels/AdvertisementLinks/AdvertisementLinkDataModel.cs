using Flvt.Domain.AdvertisementLinks;
using Flvt.Infrastructure.Data.DataModels.Filters;
using System.Xml.Linq;
using Amazon.DynamoDBv2.DocumentModel;
using Flvt.Infrastructure.Data.Extensions;

namespace Flvt.Infrastructure.Data.DataModels.AdvertisementLinks;

internal class AdvertisementLinkDataModel : IDataModel<AdvertisementLink>
{
    public string Link { get; init; }

    private AdvertisementLinkDataModel(AdvertisementLink link)
    {
        Link = link.Link;
    }
    private AdvertisementLinkDataModel(Document doc)
    {
        Link = doc.GetProperty(nameof(Link));
    }

    public static AdvertisementLinkDataModel FromDomainModel(AdvertisementLink domainModel) => new(domainModel);

    public static AdvertisementLinkDataModel FromDocument(Document document) => new(document);

    public Type GetDomainModelType() => typeof(AdvertisementLink);

    public AdvertisementLink ToDomainModel() => new (Link);
}