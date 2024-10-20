using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ProcessedAdvertisements;

namespace Flvt.Application.Custody.RemoveDuplicateAdvertisements;

internal sealed class RemoveDuplicateAdvertisementsCommandHandler : ICommandHandler<RemoveDuplicateAdvertisementsCommand>
{
    private readonly ICustodian _custodian;
    private readonly IProcessedAdvertisementRepository _processedAdvertisementRepository;

    public RemoveDuplicateAdvertisementsCommandHandler(ICustodian custodian, IProcessedAdvertisementRepository processedAdvertisementRepository)
    {
        _custodian = custodian;
        _processedAdvertisementRepository = processedAdvertisementRepository;
    }

    public async Task<Result> Handle(RemoveDuplicateAdvertisementsCommand request, CancellationToken cancellationToken)
    {
        var duplicatesLinks = await _custodian.FindDuplicateAdvertisementsAsync();

        return await _processedAdvertisementRepository.RemoveRangeAsync(duplicatesLinks);
    }
}
