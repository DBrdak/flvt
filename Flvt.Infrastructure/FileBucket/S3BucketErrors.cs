using Flvt.Domain.Primitives.Responses;

namespace Flvt.Infrastructure.FileBucket;

internal sealed class S3BucketErrors
{
    public static Error WriteFailed => new ("Failed to write file to S3 bucket");
    public static Error ReadFailed => new("Failed to read file from S3 bucket");
}