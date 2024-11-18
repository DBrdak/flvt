using Flvt.Domain.Primitives.Responses;
using Flvt.Infrastructure.Data;
using Flvt.Infrastructure.Data.DataModels;
using Flvt.Infrastructure.Data.Repositories;
using Flvt.Infrastructure.Scrapers.Domiporta;

namespace Flvt.Infrastructure.Scrapers.Shared.Helpers.Repositories;

internal sealed class ScraperHelperRepository : Repository<ScraperHelper>, IScraperHelperRepository
{
    public ScraperHelperRepository(DataContext context, DataModelService<ScraperHelper> dataModelService) : base(context, dataModelService)
    {
    }

    public async Task<Result<ScraperHelper>> GetDomiportaLatestAdvertisementHelperAsync() => 
        await GetByIdAsync(nameof(DomiportaLatestAdvertisementHelper));
}