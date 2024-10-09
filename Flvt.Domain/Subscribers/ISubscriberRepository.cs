using Flvt.Domain.Primitives.Responses;

namespace Flvt.Domain.Subscribers;

public interface ISubscriberRepository
{
    Task<Result<Subscriber>> GetByEmailAsync(string email);
    Task<Result<Subscriber>> AddAsync(Subscriber subscriber, CancellationToken cancellationToken = default);
}