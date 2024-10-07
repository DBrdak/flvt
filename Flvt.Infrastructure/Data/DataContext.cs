using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using Flvt.Domain;

namespace Flvt.Infrastructure.Data;

internal sealed class DataContext
{
    private readonly AmazonDynamoDBClient _client = new(new BasicAWSCredentials(XD.LOL2, XD.LOL3)); //TODO remove hardcoded credentials

    private readonly AmazonDynamoDBException _connectionException =
        new("Could not connect to DynamoDB");
    private AmazonDynamoDBException InvalidTableException(string typeName) =>
        new($"Table for {typeName} does not exist");

    private Table ProcessedAdvertisements => Table.TryLoadTable(_client, nameof(ProcessedAdvertisements), out var table) ?
        table : throw _connectionException;
    private Table Subscribers => Table.TryLoadTable(_client, nameof(Subscribers), out var table) ?
        table : throw _connectionException;

    public Table Set<TEntity>() =>
        typeof(TEntity) switch
        {
            { Name: "ProcessedAdvertisement" } => ProcessedAdvertisements,
            { Name: "Subscriber" } => Subscribers,
            var type => throw InvalidTableException(type.Name)
        };
}