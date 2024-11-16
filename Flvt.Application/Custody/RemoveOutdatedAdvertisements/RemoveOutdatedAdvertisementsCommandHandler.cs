using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ProcessedAdvertisements;
using Flvt.Domain.ScrapedAdvertisements;

namespace Flvt.Application.Custody.RemoveOutdatedAdvertisements;

internal sealed class RemoveOutdatedAdvertisementsCommandHandler : ICommandHandler<RemoveOutdatedAdvertisementsCommand>
{
    private readonly IProcessedAdvertisementRepository _processedAdvertisementRepository;
    private readonly ICustodian _custodian;
    private const int checkLimit = 512;

    public RemoveOutdatedAdvertisementsCommandHandler(
        IProcessedAdvertisementRepository processedAdvertisementRepository,
        ICustodian custodian)
    {
        _processedAdvertisementRepository = processedAdvertisementRepository;
        _custodian = custodian;
    }

    public async Task<Result> Handle(RemoveOutdatedAdvertisementsCommand request, CancellationToken cancellationToken)
    {
        var adsLinksGetResult =
            await _processedAdvertisementRepository.GetAdvertisementsLinksForDateCheckAsync(checkLimit);

        if (adsLinksGetResult.IsFailure)
        {
            return adsLinksGetResult.Error;
        }

        var adsLinks = adsLinksGetResult.Value.ToList();

        var outdatedAdsLinks = (await _custodian.FindOutdatedAdvertisementsAsync(adsLinks)).ToList();

        var removeTask = _processedAdvertisementRepository.RemoveRangeAsync(outdatedAdsLinks);

        var upToDateAdsLinks = adsLinks.Except(outdatedAdsLinks);

        var upToDateAdsGetResult = await _processedAdvertisementRepository.GetManyByLinkAsync(upToDateAdsLinks);

        if (upToDateAdsGetResult.IsFailure)
        {
            return upToDateAdsGetResult.Error;
        }

        var upToDateAds = upToDateAdsGetResult.Value.ToList();

        upToDateAds.ForEach(ad => ad.CheckedForOutdate());

        var updateTask = _processedAdvertisementRepository.UpdateRangeAsync(upToDateAds);

        return Result.Aggregate(await Task.WhenAll(removeTask, updateTask));
    }
}
