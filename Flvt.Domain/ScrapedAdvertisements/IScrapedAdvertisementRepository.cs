using Flvt.Domain.Primitives.Responses;

namespace Flvt.Domain.ScrapedAdvertisements;

public interface IScrapedAdvertisementRepository
{
    Task<Result<IEnumerable<ScrapedAdvertisement>>> GetAllAsync();
    Task<Result<IEnumerable<ScrapedAdvertisement>>> GetManyByLinkAsync(IEnumerable<string> links);
    Task<Result> AddRangeAsync(IEnumerable<ScrapedAdvertisement> advertisements);
    Task<Result> UpdateRangeAsync(IEnumerable<ScrapedAdvertisement> advertisements);
    Task<Result<IEnumerable<ScrapedAdvertisement>>> GetUnprocessedAsync();

    void StartBatchWrite();
    void AddItemToBatchWrite(ScrapedAdvertisement scrapedAdvertisement);
    void AddManyItemsToBatchWrite(IEnumerable<ScrapedAdvertisement> scrapedAdvertisement);
    Task<Result> ExecuteBatchWriteAsync();

    Task<Result> RemoveRangeAsync(IEnumerable<string> scrapedAdvertisementsIds);

    Task<Result<IEnumerable<ScrapedAdvertisement>>> GetAdvertisementsInProcessAsync();

    Task<Result<IEnumerable<string>>> GetAllLinksAsync();
}