using Flvt.Application.Abstractions;
using Flvt.Application.Advertisements.Models;
using Flvt.Domain.Filters;
using Flvt.Domain.Photos;
using Flvt.Domain.ProcessedAdvertisements;
using Serilog;

namespace Flvt.Application.Subscribers.Services;

internal sealed class FiltersService
{
    private readonly IProcessedAdvertisementRepository _processedAdvertisementRepository;
    private readonly IAdvertisementPhotosRepository _photosRepository;
    private readonly IFileService _fileService;

    public FiltersService(
        IProcessedAdvertisementRepository processedAdvertisementRepository,
        IAdvertisementPhotosRepository photosRepository,
        IFileService fileService)
    {
        _processedAdvertisementRepository = processedAdvertisementRepository;
        _photosRepository = photosRepository;
        _fileService = fileService;
    }

    public async Task<Filter?> LaunchFilter(Filter filter)
    {
        var advertisementsGetResult = await _processedAdvertisementRepository.GetByFilterAsync(filter);

        if (advertisementsGetResult.IsFailure)
        {
            Log.Error(
                "Failed to get advertisements for filter {FilterId}, error :{error}",
                filter.Id,
                advertisementsGetResult.Error);
            return null;
        }

        var advertisements = advertisementsGetResult.Value.ToList();

        var advertisementLinks = advertisements
            .Select(advertisement => advertisement.Link)
            .ToList();

        var photosGetResult = await _photosRepository.GetByManyAdvertisementLinkAsync(advertisementLinks);

        if (photosGetResult.IsFailure)
        {
            Log.Error(
                "Failed to get advertisements photos for filter {FilterId}, error :{error}",
                filter.Id,
                photosGetResult.Error);
            return null;
        }
        var photos = photosGetResult.Value.ToList();

        filter.NewAdvertisementsFound(advertisementLinks);

        var advertisementsToFile = ProcessedAdvertisementModel.FromFilter(filter, advertisements, photos);

        var advertisementsFileWriteResult = await _fileService.WriteAdvertisementsToFileAsync(filter, advertisementsToFile);

        if (advertisementsFileWriteResult.IsFailure)
        {
            Log.Error(
                "Failed to write advertisements to file for filter {FilterId}, error :{error}",
                filter.Id,
                advertisementsFileWriteResult.Error);
            return null;
        }

        var advertisementsFilePath = advertisementsFileWriteResult.Value;

        filter.NewAdvertisementsSavedToFile(advertisementsFilePath);

        return filter;
    }
}