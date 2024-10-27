using System.Text;
using Flvt.Application.Advertisements.Models;
using Flvt.Domain.Filters;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ProcessedAdvertisements;
using Newtonsoft.Json;

namespace Flvt.Infrastructure.FileBucket;

internal sealed class FileService
{
    private readonly S3Bucket _s3Bucket;

    public FileService(S3Bucket s3Bucket)
    {
        _s3Bucket = s3Bucket;
    }

    public async Task<Result<string>> WriteAdvertisementsToFileAsync(
        Filter filter,
        IEnumerable<ProcessedAdvertisementModel> advertisements)
    {
        var advertisementsJson = JsonConvert.SerializeObject(advertisements);

        var filePath = $"ads/{filter.Id}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.json";

        var response = await _s3Bucket.WriteFileAsync(
            Encoding.UTF8.GetBytes(advertisementsJson),
            filePath,
            "application/json");

        if (response.IsFailure)
        {
            return response.Error;
        }

        return filePath;
    }

    public async Task<Result<string>> GetAdvertisementsUrlAsync(Filter filter)
    {
        if (string.IsNullOrWhiteSpace(filter.AdvertisementsFilePath) ||
            !Path.IsPathFullyQualified(filter.AdvertisementsFilePath))
        {
            return FileServiceErrors.InvalidFilePath;
        }

        return await _s3Bucket.GetFileUrlAsync(filter.AdvertisementsFilePath ?? string.Empty);
    }
}