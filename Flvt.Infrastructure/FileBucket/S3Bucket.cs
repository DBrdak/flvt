using Amazon.S3;
using Amazon.S3.Model;
using Flvt.Domain.Primitives.Responses;
using Flvt.Infrastructure.AWS.Contants;
using Flvt.Infrastructure.Utlis.Extensions;

namespace Flvt.Infrastructure.FileBucket;

internal sealed class S3Bucket
{
    private readonly IAmazonS3 _s3Client;

    public S3Bucket(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }

    public async Task<Result> WriteFileAsync(byte[] fileBytes, string bucketFilePath, string contentType = "plain/text")
    {
        var stream = new MemoryStream(fileBytes);

        var putRequest = new PutObjectRequest
        {
            BucketName = CloudEnvironment.BucketName,
            Key = bucketFilePath,
            InputStream = stream,
            ContentType = contentType
        };

        var response = await _s3Client.PutObjectAsync(putRequest);

        await stream.DisposeAsync();

        return response.HttpStatusCode.IsSuccessStatusCode() ? Result.Success() : S3BucketErrors.WriteFailed;
    }

    public async Task<Result<byte[]>> ReadFileAsync(string bucketFilePath)
    {
        var getRequest = new GetObjectRequest
        {
            BucketName = CloudEnvironment.BucketName,
            Key = bucketFilePath,
        };

        using var response = await _s3Client.GetObjectAsync(getRequest);

        if (!response.HttpStatusCode.IsSuccessStatusCode())
        {
            return S3BucketErrors.ReadFailed;
        }

        using var memoryStream = new MemoryStream();
        await response.ResponseStream.CopyToAsync(memoryStream);

        return memoryStream.ToArray();
    }

    public async Task<string> GetFileUrlAsync(string bucketFilePath)
    {
        var getRequest = new GetPreSignedUrlRequest
        {
            BucketName = CloudEnvironment.BucketName,
            Key = bucketFilePath,
            Expires = DateTime.UtcNow.AddMinutes(5)
        };

        return await _s3Client.GetPreSignedURLAsync(getRequest);
    }
}