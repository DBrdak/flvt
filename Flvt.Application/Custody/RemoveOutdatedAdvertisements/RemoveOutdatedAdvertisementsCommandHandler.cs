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

        var advertisementsLinks = processedAdvertisementsGetResult.Result.Value
            .Select(ad => ad.Link)
            .ToList();
        advertisementsLinks.AddRange(scrapedAdvertisementsGetResult.Result.Value
            .Select(ad => ad.Link));

        var outdatedAdvertisements = (await _custodian.FindOutdatedAdvertisementsAsync(advertisementsLinks)).ToList();

        IEnumerable<Task<Result>> removeTasks =
        [
            _processedAdvertisementRepository.RemoveRangeAsync(outdatedAdvertisements),
            _scrapedAdvertisementRepository.RemoveRangeAsync(outdatedAdvertisements)
        ];

        return Result.Aggregate(await Task.WhenAll(removeTasks));
    }
}
