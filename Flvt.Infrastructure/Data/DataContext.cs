using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using Flvt.Domain;
using Flvt.Domain.ProcessedAdvertisements;
using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Domain.Subscribers;
using Flvt.Infrastructure.AWS.Contants;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.DataModels.Batches;

namespace Flvt.Infrastructure.Data;

internal sealed class DataContext
{
    private readonly AmazonDynamoDBClient _client = new (
        /*new BasicAWSCredentials(XD.LOL2, XD.LOL3), CloudEnvironment.RegionEndpoint*/); //TODO remove hardcoded credentials

    private readonly AmazonDynamoDBException _connectionException =
        new("Could not connect to DynamoDB");
    private AmazonDynamoDBException InvalidTableException(string typeName) =>
        new($"Table for {typeName} does not exist");

    private Table ProcessedAdvertisements => Table.TryLoadTable(_client, nameof(ProcessedAdvertisements), out var table) ?
        table : throw _connectionException;
    private Table ScrapedAdvertisements => Table.TryLoadTable(_client, nameof(ScrapedAdvertisements), out var table) ?
        table : throw _connectionException;
    private Table Subscribers => Table.TryLoadTable(_client, nameof(Subscribers), out var table) ?
        table : throw _connectionException;
    private Table Batches => Table.TryLoadTable(_client, nameof(Batches), out var table) ?
        table : throw _connectionException;

    public Table Set<TEntity>() =>
        typeof(TEntity) switch
        {
            { Name: nameof(ProcessedAdvertisement) } => ProcessedAdvertisements,
            { Name: nameof(ScrapedAdvertisement) } => ScrapedAdvertisements,
            { Name: nameof(Subscriber) } => Subscribers,
            { Name: nameof(BatchDataModel) } => Batches,
            var type => throw InvalidTableException(type.Name)
        };
}