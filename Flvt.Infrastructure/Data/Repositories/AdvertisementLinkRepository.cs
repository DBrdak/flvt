using Flvt.Domain.AdvertisementLinks;
using Flvt.Infrastructure.Data.DataModels;

namespace Flvt.Infrastructure.Data.Repositories;

internal sealed class AdvertisementLinkRepository : Repository<AdvertisementLink>, IAdvertisementLinkRepository
{
    public AdvertisementLinkRepository(DataContext context, DataModelService<AdvertisementLink> dataModelService) : base(context, dataModelService)
    {
    }
}