using Flvt.Domain.Filters;
using Flvt.Domain.Primitives.Responses;

namespace Flvt.Domain.ProcessedAdvertisements;

public interface IProcessedAdvertisementRepository
{
    Task<Result<IEnumerable<ProcessedAdvertisement>>> GetAllAsync();
    Task<Result<IEnumerable<ProcessedAdvertisement>>> GetManyByLinkAsync(IEnumerable<string> links);

    Task<Result<IEnumerable<string>>> GetAdvertisementsLinksForDateCheckAsync(int limit);
    Task<Result> AddRangeAsync(IEnumerable<ProcessedAdvertisement> advertisements);

    Task<Result> RemoveRangeAsync(IEnumerable<string> advertisementsLinks);
    void StartBatchGet();
    void AddManyItemsToBatchGet(IEnumerable<string> ids);
    void AddItemToBatchGet(string scrapedAdvertisementId);
    Task<Result<IEnumerable<ProcessedAdvertisement>>> ExecuteBatchGetAsync();

    Task<Result<ProcessedAdvertisement>> GetByLinkAsync(string link);

    Task<Result<ProcessedAdvertisement>> UpdateAsync(ProcessedAdvertisement advertisement);

    Task<Result<IEnumerable<ProcessedAdvertisement>>> GetByFilterAsync(Filter filter);

    Task<Result> UpdateRangeAsync(IEnumerable<ProcessedAdvertisement> ads);

    Task<Result<IEnumerable<string>>> GetAllLinksAsync();

    Task<Result<IEnumerable<string>>> GetAllDedupesAsync();
}