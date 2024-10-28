using Flvt.Application.Advertisements.Models;
using Flvt.Domain.Filters;
using Flvt.Domain.Primitives.Responses;

namespace Flvt.Application.Abstractions;

public interface IFileService
{
    Task<Result<string>> WriteAdvertisementsToFileAsync(
        Filter filter,
        IEnumerable<ProcessedAdvertisementModel> advertisements);

    Task<Result<string>> GetAdvertisementsUrlAsync(Filter filter);
}