using Flvt.Domain.Primitives.Responses;

namespace Flvt.Domain.Subscribers;

public interface ISubscriberRepository
{
    Task<Result<Subscriber>> GetByEmailAsync(string email);
    Task<Result<Subscriber>> AddAsync(Subscriber subscriber);

    Task<Result<Subscriber>> UpdateAsync(Subscriber subscriber);

    Task<Result<IEnumerable<Subscriber>>> GetManyByEmailAsync(IEnumerable<string> emails);

    Task<Result> RemoveAsync(string subscriberEmail);
}