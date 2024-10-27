using Flvt.Domain.Primitives.Responses;

namespace Flvt.Infrastructure.FileBucket;

internal sealed class FileServiceErrors
{
    public static readonly Error InvalidFilePath = new("Invalid file path");
}