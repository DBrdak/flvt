using System.Security.Principal;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Flvt.Domain.Advertisements;

namespace Flvt.Infrastructure.Data;

internal sealed class DataContext
{
    private readonly AmazonDynamoDBClient _client = new();

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
            { Name: "ProcessedAdvertisements" } => ProcessedAdvertisements,
            { Name: "Subscribers" } => Subscribers,
            var type => throw InvalidTableException(type.Name)
        };
}