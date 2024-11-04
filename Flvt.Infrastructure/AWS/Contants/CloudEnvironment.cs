using Amazon;

namespace Flvt.Infrastructure.AWS.Contants;

internal static class CloudEnvironment
{
    public const string Region = "eu-west-1";
    public static readonly RegionEndpoint RegionEndpoint = RegionEndpoint.EUWest1;
    public const string BucketName = "flvt";
}