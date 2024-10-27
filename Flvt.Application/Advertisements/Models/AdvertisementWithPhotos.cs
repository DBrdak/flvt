using Flvt.Domain.ProcessedAdvertisements;

namespace Flvt.Application.Advertisements.Models;

public sealed record AdvertisementWithPhotos(ProcessedAdvertisement Advertisement, IEnumerable<string> Photos);