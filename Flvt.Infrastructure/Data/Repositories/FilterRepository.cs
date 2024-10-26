using Flvt.Domain.Filters;
using Flvt.Infrastructure.Data.DataModels;

namespace Flvt.Infrastructure.Data.Repositories;

internal sealed class FilterRepository : Repository<Filter>, IFilterRepository
{
    public FilterRepository(
        DataContext context,
        DataModelService<Filter> dataModelService) : base(
        context,
        dataModelService)
    {
    }
}