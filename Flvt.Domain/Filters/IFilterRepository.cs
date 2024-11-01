using Flvt.Domain.Primitives.Responses;

namespace Flvt.Domain.Filters;

public interface IFilterRepository
{
    Task<Result<Filter>> GetByIdAsync(string id);
    Task<Result<IEnumerable<Filter>>> GetAllAsync();
    Task<Result<Filter>> AddAsync(Filter filter);
    Task<Result<Filter>> UpdateAsync(Filter filter);
    void StartBatchWrite();
    void AddItemToBatchWrite(Filter filter);
    Task<Result> ExecuteBatchWriteAsync();

    Task<Result<IEnumerable<Filter>>> GetManyByIdAsync(IEnumerable<string> filtersIds);

    Task<Result> RemoveAsync(string filterId);
}