using Flvt.Application.Messaging;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ProcessedAdvertisements;

namespace Flvt.Application.Advertisements.Flag;

internal sealed class FlagCommandHandler : ICommandHandler<FlagCommand>
{
    private readonly IProcessedAdvertisementRepository _processedAdvertisementRepository;

    public FlagCommandHandler(IProcessedAdvertisementRepository processedAdvertisementRepository)
    {
        _processedAdvertisementRepository = processedAdvertisementRepository;
    }

    public async Task<Result> Handle(FlagCommand request, CancellationToken cancellationToken)
    {
        var advertisementGetResult =
            await _processedAdvertisementRepository.GetByLinkAsync(request.AdvertisementLink);

        if (advertisementGetResult.IsFailure)
        {
            return advertisementGetResult.Error;
        }

        var advertisement = advertisementGetResult.Value;

        advertisement.Flag();

        return await _processedAdvertisementRepository.UpdateAsync(advertisement);
    }
}
