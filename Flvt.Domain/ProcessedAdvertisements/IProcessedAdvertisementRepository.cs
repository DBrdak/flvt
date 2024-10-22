using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ScrapedAdvertisements;

namespace Flvt.Domain.ProcessedAdvertisements;

public interface IProcessedAdvertisementRepository
{
    Task<Result<IEnumerable<ProcessedAdvertisement>>> GetAllAsync();
    Task<Result<IEnumerable<ProcessedAdvertisement>>> GetManyByLinkAsync(IEnumerable<string> links);
    Task<Result> AddRangeAsync(IEnumerable<ProcessedAdvertisement> advertisements);

    Task<Result> RemoveRangeAsync(IEnumerable<string> advertisementsLinks);
    void StartBatchGet();
    void AddItemToBatchGet(string scrapedAdvertisementId);
    Task<Result<IEnumerable<ProcessedAdvertisement>>> ExecuteBatchGetAsync();
}