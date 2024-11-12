using Flvt.Domain.Filters;
using Flvt.Domain.Primitives.Responses;

namespace Flvt.Application.Abstractions;

public interface IQueuePublisher
{
    Task<Result> PublishFinishedBatches();

    Task<Result> PublishLaunchedFilters(List<Filter> launchedFilters);
    Task<Result> PublishScrapedLinksAsync(List<string> scrapedLinks);
}