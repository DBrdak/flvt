using Amazon.DynamoDBv2.DocumentModel;

namespace Flvt.Infrastructure.Data.Extensions;

public static class DynamoDbEntryExtensions
{
    public static string? AsNullableString(this DynamoDBEntry entry) =>
        Equals(entry, DynamoDBNull.Null) ? null : entry.AsString();

    public static long? AsNullableLong(this DynamoDBEntry entry) =>
        Equals(entry, DynamoDBNull.Null) ? null : entry.AsLong();
    public static int? AsNullableInt(this DynamoDBEntry entry) =>
        Equals(entry, DynamoDBNull.Null) ? null : entry.AsInt();

    public static decimal? AsNullableDecimal(this DynamoDBEntry entry) =>
        Equals(entry, DynamoDBNull.Null) ? null : entry.AsDecimal();

    public static Guid? AsNullableGuid(this DynamoDBEntry entry) =>
        Equals(entry, DynamoDBNull.Null) ? null : Guid.Parse(entry.AsString());
    public static bool? AsNullableBoolean(this DynamoDBEntry entry) =>
        Equals(entry, DynamoDBNull.Null) ? null : entry.AsBoolean();
    public static DynamoDBEntry? GetNullableProperty(this Document doc, string propertyName) =>
        doc.TryGetValue(propertyName, out var entry) ? entry : null;
    public static DynamoDBEntry GetProperty(this Document doc, string propertyName) =>
        doc.TryGetValue(
            propertyName,
            out var entry) ?
            entry :
            throw new NullReferenceException($"DynamoDB entry not found: {propertyName}");
}