using Flvt.Domain.Primitives.Responses;

namespace Flvt.Domain.Photos;

public interface IAdvertisementPhotosRepository
{
    Task<Result<AdvertisementPhotos>> GetByAdvertisementLinkAsync(string link);
    Task<Result<IEnumerable<AdvertisementPhotos>>> GetByManyAdvertisementLinkAsync(IEnumerable<string> link);
    Task<Result> AddRangeAsync(IEnumerable<AdvertisementPhotos> advertisementPhotos);

    Task<Result<IEnumerable<AdvertisementPhotos>>> GetAllAsync();

    Task<Result> RemoveRangeAsync(IEnumerable<string> photosToRemove);
}