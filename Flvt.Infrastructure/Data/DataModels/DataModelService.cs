using Amazon.DynamoDBv2.DocumentModel;
using Flvt.Domain.AdvertisementLinks;
using Flvt.Domain.Photos;
using Flvt.Domain.ProcessedAdvertisements;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Domain.Subscribers;
using Flvt.Infrastructure.Data.DataModels.AdvertisementLinks;
using Flvt.Infrastructure.Data.DataModels.Batches;
using Flvt.Infrastructure.Data.DataModels.Filters;
using Flvt.Infrastructure.Data.DataModels.Photos;
using Flvt.Infrastructure.Data.DataModels.ProcessedAdvertisements;
using Flvt.Infrastructure.Data.DataModels.ScrapedAdvertisements;
using Flvt.Infrastructure.Data.DataModels.ScraperHelpers;
using Flvt.Infrastructure.Data.DataModels.Subscribers;
using Flvt.Infrastructure.Scrapers.Shared.Helpers;

namespace Flvt.Infrastructure.Data.DataModels;

internal sealed class DataModelService<TEntity>
{
    private readonly InvalidCastException _convertDomainModelToDataModelException =
        new($"Cannot cast {typeof(TEntity).Name} to data model");
    private readonly InvalidCastException _convertDocumentToDataModelException =
        new($"Cannot cast document to data model of {typeof(TEntity).Name}");

    public IDataModel<TEntity> ConvertDomainModelToDataModel(TEntity entity) =>
        entity switch
        {
            ProcessedAdvertisement processedAdvertisement => ProcessedAdvertisementDataModel
                .FromDomainModel(processedAdvertisement) as IDataModel<TEntity>,
            ScrapedAdvertisement scrapedAdvertisement => ScrapedAdvertisementDataModel
                .FromDomainModel(scrapedAdvertisement) as IDataModel<TEntity>,
            Subscriber subscriber => SubscriberDataModel
                .FromDomainModel(subscriber) as IDataModel<TEntity>,
            BatchDataModel batchDataModel => BatchDataModel
                .FromDomainModel(batchDataModel) as IDataModel<TEntity>,
            Domain.Filters.Filter filter => FilterDataModel
                .FromDomainModel(filter) as IDataModel<TEntity>,
            AdvertisementPhotos photos => AdvertisementPhotosDataModel
                .FromDomainModel(photos) as IDataModel<TEntity>,
            ScraperHelper scraperHelper => ScraperHelperDataModel
                .FromDomainModel(scraperHelper) as IDataModel<TEntity>,
            AdvertisementLink advertisementLink => AdvertisementLinkDataModel
                .FromDomainModel(advertisementLink) as IDataModel<TEntity>,
            _ => throw _convertDomainModelToDataModelException
        } ??
        throw _convertDomainModelToDataModelException;

    public IDataModel<TEntity> ConvertDocumentToDataModel(Document doc) =>
        typeof(TEntity) switch
        {
            { Name: nameof(ProcessedAdvertisement) } =>
                ProcessedAdvertisementDataModel.FromDocument(doc) as IDataModel<TEntity>,
            { Name : nameof(ScrapedAdvertisement) } =>
                ScrapedAdvertisementDataModel.FromDocument(doc) as IDataModel<TEntity>,
            { Name: nameof(Filter) } =>
                FilterDataModel.FromDocument(doc) as IDataModel<TEntity>,
            { Name: nameof(Subscriber) } =>
                SubscriberDataModel.FromDocument(doc) as IDataModel<TEntity>,
            { Name: nameof(BatchDataModel) } =>
                BatchDataModel.FromDocument(doc) as IDataModel<TEntity>,
            { Name: nameof(AdvertisementPhotos) } =>
                AdvertisementPhotosDataModel.FromDocument(doc) as IDataModel<TEntity>,
            { Name: nameof(ScraperHelper) } =>
                ScraperHelperDataModel.FromDocument(doc) as IDataModel<TEntity>,
            { Name: nameof(AdvertisementLink) } =>
                AdvertisementLinkDataModel.FromDocument(doc) as IDataModel<TEntity>,
            _ => throw _convertDocumentToDataModelException
        } ??
        throw _convertDocumentToDataModelException;
}