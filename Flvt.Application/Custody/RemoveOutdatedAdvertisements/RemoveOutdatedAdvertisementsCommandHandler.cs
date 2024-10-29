using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ProcessedAdvertisements;
using Flvt.Domain.ScrapedAdvertisements;

namespace Flvt.Application.Custody.RemoveOutdatedAdvertisements;

internal sealed class RemoveOutdatedAdvertisementsCommandHandler : ICommandHandler<RemoveOutdatedAdvertisementsCommand>
{
    private readonly IProcessedAdvertisementRepository _processedAdvertisementRepository;
    private readonly IScrapedAdvertisementRepository _scrapedAdvertisementRepository;
    private readonly ICustodian _custodian;

    public RemoveOutdatedAdvertisementsCommandHandler(
        IProcessedAdvertisementRepository processedAdvertisementRepository,
        ICustodian custodian,
        IScrapedAdvertisementRepository scrapedAdvertisementRepository)
    {
        _processedAdvertisementRepository = processedAdvertisementRepository;
        _custodian = custodian;
        _scrapedAdvertisementRepository = scrapedAdvertisementRepository;
    }

    public async Task<Result> Handle(RemoveOutdatedAdvertisementsCommand request, CancellationToken cancellationToken)
    {
        var processedAdvertisementsGetResult = _processedAdvertisementRepository.GetAllAsync();
        var scrapedAdvertisementsGetResult = _scrapedAdvertisementRepository.GetAllAsync();

        await Task.WhenAll(processedAdvertisementsGetResult, scrapedAdvertisementsGetResult);

        if (processedAdvertisementsGetResult.Result.IsFailure)
        {
            return processedAdvertisementsGetResult.Result.Error;
        }

        if (scrapedAdvertisementsGetResult.Result.IsFailure)
        {
            return scrapedAdvertisementsGetResult.Result.Error;
        }

        var processedAdvertisements = processedAdvertisementsGetResult.Result.Value;
        var scrapedAdvertisements = scrapedAdvertisementsGetResult.Result.Value.ToList();

        var advertisementsLinks = processedAdvertisements
            .Select(ad => ad.Link)
            .ToList();
        advertisementsLinks.AddRange(scrapedAdvertisements
            .Select(ad => ad.Link));

        var outdatedAdvertisements = (await _custodian.FindOutdatedAdvertisementsAsync(advertisementsLinks)).ToList();

        outdatedAdvertisements.AddRange(
            scrapedAdvertisements
                .Where(ad => string.IsNullOrWhiteSpace(ad.AdContent))
                .Select(ad => ad.Link));

        IEnumerable<Task<Result>> removeTasks =
        [
            _processedAdvertisementRepository.RemoveRangeAsync(outdatedAdvertisements),
            _scrapedAdvertisementRepository.RemoveRangeAsync(outdatedAdvertisements)
        ];

        return Result.Aggregate(await Task.WhenAll(removeTasks));
    }
}
