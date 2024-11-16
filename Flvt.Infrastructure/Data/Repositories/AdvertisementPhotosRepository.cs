using Flvt.Domain.Photos;
using Flvt.Domain.Primitives.Responses;
using Flvt.Infrastructure.Data.DataModels;
using Flvt.Infrastructure.Data.DataModels.Photos;
using Flvt.Infrastructure.Data.Extensions;

namespace Flvt.Infrastructure.Data.Repositories;

internal sealed class AdvertisementPhotosRepository : Repository<AdvertisementPhotos>, IAdvertisementPhotosRepository
{
    public AdvertisementPhotosRepository(DataContext context, DataModelService<AdvertisementPhotos> dataModelService) : base(context, dataModelService)
    {
    }

    public async Task<Result<AdvertisementPhotos>> GetByAdvertisementLinkAsync(string link) =>
        await GetByIdAsync(link);

    public async Task<Result<IEnumerable<AdvertisementPhotos>>> GetByManyAdvertisementLinkAsync(
        IEnumerable<string> link) =>
        await GetManyByIdAsync(link);

    public async Task<Result<IEnumerable<string>>> GetAllAdvertisementsLinksAsync()
    {
        var getResult = await GetAllAsync(null, [nameof(AdvertisementPhotosDataModel.AdvertisementLink)]);

        return getResult.IsSuccess
            ?
            Result.Success(
                getResult.Value.Select(
                    doc => doc.GetProperty(nameof(AdvertisementPhotosDataModel.AdvertisementLink))
                        .AsString()))
            : Result.Failure<IEnumerable<string>>(getResult.Error);
    }
}