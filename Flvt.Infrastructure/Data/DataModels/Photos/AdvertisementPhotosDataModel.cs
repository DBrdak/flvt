using Amazon.DynamoDBv2.DocumentModel;
using Flvt.Domain.Photos;
using Flvt.Infrastructure.Data.Extensions;

namespace Flvt.Infrastructure.Data.DataModels.Photos;

internal sealed class AdvertisementPhotosDataModel : IDataModel<AdvertisementPhotos>
{
    public string AdvertisementLink { get; set; }
    public IEnumerable<string> Links { get; set; }

    private AdvertisementPhotosDataModel(AdvertisementPhotos original)
    {
        AdvertisementLink = original.AdvertisementLink;
        Links = original.Links;
    }

    private AdvertisementPhotosDataModel(Document doc)
    {
        AdvertisementLink = doc.GetProperty(nameof(AdvertisementLink));
        Links = doc.GetProperty(nameof(Links)).AsArrayOfString();
    }

    public static AdvertisementPhotosDataModel FromDomainModel(AdvertisementPhotos original) =>
        new(original);

    public static AdvertisementPhotosDataModel FromDocument(Document doc) => new(doc);

    public Type GetDomainModelType() => typeof(AdvertisementPhotos);

    public AdvertisementPhotos ToDomainModel() => 
        new(AdvertisementLink, Links);
}