using System.Text;
using System.Text.RegularExpressions;
using Flvt.Application.Abstractions;
using Flvt.Application.Advertisements.Models;
using Flvt.Domain.Filters;
using Flvt.Domain.Primitives.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Flvt.Infrastructure.FileBucket;

internal sealed class FileService : IFileService
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
        var advertisementsJson = JsonConvert.SerializeObject(
            advertisements,
            Formatting.None,
            new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });


        var filePath = $"ads/{filter.Id}.json";

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
        if ((string.IsNullOrWhiteSpace(filter.AdvertisementsFilePath) ||
            !Regex.IsMatch(filter.AdvertisementsFilePath, FileBucketConstants.S3FilePathPattern)) &&
            filter.AdvertisementsFilePath is not null && !filter.AdvertisementsFilePath.Contains("preview"))
        {
            return FileServiceErrors.InvalidFilePath;
        }

        return await _s3Bucket.GetFileUrlAsync(filter.AdvertisementsFilePath ?? string.Empty);
    }
}