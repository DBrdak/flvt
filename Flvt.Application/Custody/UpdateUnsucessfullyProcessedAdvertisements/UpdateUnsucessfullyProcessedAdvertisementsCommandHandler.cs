using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ScrapedAdvertisements;

namespace Flvt.Application.Custody.UpdateUnsucessfullyProcessedAdvertisements;

internal sealed class UpdateUnsucessfullyProcessedAdvertisementsCommandHandler :
    ICommandHandler<UpdateUnsucessfullyProcessedAdvertisementsCommand>
{
    private readonly ICustodian _custodian;
    private readonly IScrapedAdvertisementRepository _scrapedAdvertisementRepository;

    public UpdateUnsucessfullyProcessedAdvertisementsCommandHandler(
        ICustodian custodian,
        IScrapedAdvertisementRepository scrapedAdvertisementRepository)
    {
        _custodian = custodian;
        _scrapedAdvertisementRepository = scrapedAdvertisementRepository;
    }

    public async Task<Result> Handle(
        UpdateUnsucessfullyProcessedAdvertisementsCommand request,
        CancellationToken cancellationToken)
    {
        var scrapedAdvertisementsGetResult = await _scrapedAdvertisementRepository.GetAdvertisementsInProcessAsync();

        if(scrapedAdvertisementsGetResult.IsFailure)
        {
            return scrapedAdvertisementsGetResult.Error;
        }

        var scrapedAdvertisements = scrapedAdvertisementsGetResult.Value;

        scrapedAdvertisements = await _custodian.FindUnsucessfullyProcessedAdvertisements(scrapedAdvertisements);

        _scrapedAdvertisementRepository.StartBatchWrite();
        _scrapedAdvertisementRepository.AddManyItemsToBatchWrite(scrapedAdvertisements);
        await _scrapedAdvertisementRepository.ExecuteBatchWriteAsync();

        return Result.Success();
    }
}
