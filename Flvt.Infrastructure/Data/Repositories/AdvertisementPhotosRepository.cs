using Flvt.Domain.Photos;
using Flvt.Domain.Primitives.Responses;
using Flvt.Infrastructure.Data.DataModels;

namespace Flvt.Infrastructure.Data.Repositories
{
    internal sealed class AdvertisementPhotosRepository : Repository<AdvertisementPhotos>, IAdvertisementPhotosRepository
    {
        public AdvertisementPhotosRepository(DataContext context, DataModelService<AdvertisementPhotos> dataModelService) : base(context, dataModelService)
        {
        }

        public async Task<Result<AdvertisementPhotos>> GetByAdvertisementLinkAsync(string link) =>
            await GetByIdAsync(link);
    }
}
