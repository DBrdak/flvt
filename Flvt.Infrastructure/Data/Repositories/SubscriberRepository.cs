using Flvt.Domain.Primitives;
using Flvt.Domain.Subscribers;

namespace Flvt.Infrastructure.Data.Repositories;

internal sealed class SubscriberRepository : Repository<Subscriber>, ISubscriberRepository
{
    public SubscriberRepository(DataContext context) : base(context)
    {
    }

    public async Task<Result<Subscriber>> GetByEmailAsync(string email) => await GetByIdAsync(email);
}