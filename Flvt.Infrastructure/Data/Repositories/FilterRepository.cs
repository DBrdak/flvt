using Flvt.Domain.Filters;
using Flvt.Domain.Primitives.Responses;

namespace Flvt.Infrastructure.Data.Repositories;

internal sealed class FilterRepository : Repository<Filter>, IFilterRepository
{
    public FilterRepository(DataContext context) : base(context)
    {
    }
}