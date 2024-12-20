using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Domain.Photos;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ProcessedAdvertisements;
using Flvt.Domain.ScrapedAdvertisements;
using Serilog;

namespace Flvt.Application.Custody.RemoveUnusedAdvertisementsPhotos;

internal sealed class RemoveUnusedAdvertisementsPhotosCommandHandler :
    ICommandHandler<RemoveUnusedAdvertisementsPhotosCommand>
{
    private readonly ICustodian _custodian;
    private readonly IProcessedAdvertisementRepository _processedAdvertisementRepository;
    private readonly IScrapedAdvertisementRepository _scrapedAdvertisementRepository;
    private readonly IAdvertisementPhotosRepository _advertisementPhotosRepository;

    public RemoveUnusedAdvertisementsPhotosCommandHandler(
        ICustodian custodian,
        IProcessedAdvertisementRepository processedAdvertisementRepository,
        IScrapedAdvertisementRepository scrapedAdvertisementRepository,
        IAdvertisementPhotosRepository advertisementPhotosRepository)
    {
        _custodian = custodian;
        _processedAdvertisementRepository = processedAdvertisementRepository;
        _scrapedAdvertisementRepository = scrapedAdvertisementRepository;
        _advertisementPhotosRepository = advertisementPhotosRepository;
    }

    public async Task<Result> Handle(RemoveUnusedAdvertisementsPhotosCommand request, CancellationToken cancellationToken)
    {
        var processedAdvertisementsGetTask = _processedAdvertisementRepository.GetAllLinksAsync();
        var scrapedAdvertisementsGetTask = _scrapedAdvertisementRepository.GetAllLinksAsync();
        var photosGetTask = _advertisementPhotosRepository.GetAllAdvertisementsLinksAsync();

        await Task.WhenAll(processedAdvertisementsGetTask, scrapedAdvertisementsGetTask, photosGetTask);

        var processedAdvertisementsGetResult = processedAdvertisementsGetTask.Result;
        var scrapedAdvertisementsGetResult = scrapedAdvertisementsGetTask.Result;
        var photosGetResult = photosGetTask.Result;

        if (Result.Aggregate(
            [
                processedAdvertisementsGetResult,
                scrapedAdvertisementsGetResult,
                photosGetResult
            ]) is var result &&
            result.IsFailure)
        {
            return result.Error;
        }

        var advertisementsLinks = processedAdvertisementsGetResult.Value.ToList();
        advertisementsLinks.AddRange(scrapedAdvertisementsGetResult.Value);
        var photosAdvertisementsLinks = photosGetResult.Value.ToList();

        var photosToRemove = (await _custodian.FindUnusedAdvertisementPhotos(
            advertisementsLinks,
            photosAdvertisementsLinks)).ToList();
     
        Log.Logger.Information("Found {photosCount} photos to remove", photosToRemove.Count());
        
        return await _advertisementPhotosRepository.RemoveRangeAsync(photosToRemove);
    }
}
