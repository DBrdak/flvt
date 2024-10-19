using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ScrapedAdvertisements;

namespace Flvt.Application.Advertisements.ProcessAdvertisements;

internal sealed class ProcessAdvertisementsCommandHandler : ICommandHandler<ProcessAdvertisementsCommand>
{
    private readonly IScrapedAdvertisementRepository _scrapedAdvertisementRepository;
    private readonly IProcessingOrchestrator _processingOrchestrator;

    public ProcessAdvertisementsCommandHandler(IProcessingOrchestrator processingOrchestrator, IScrapedAdvertisementRepository scrapedAdvertisementRepository)
    {
        _processingOrchestrator = processingOrchestrator;
        _scrapedAdvertisementRepository = scrapedAdvertisementRepository;
    }

    public async Task<Result> Handle(ProcessAdvertisementsCommand request, CancellationToken cancellationToken)
    {
        var scrapedAdvertisementsGetResult = await _scrapedAdvertisementRepository.GetManyByLinkAsync(["https://www.otodom.pl/pl/oferta/przytulne-mieszkanie-z-ogrodkiem-48m-ID4dHBU"]);

        if (scrapedAdvertisementsGetResult.IsFailure)
        {
            return scrapedAdvertisementsGetResult.Error;
        }

        var scrapedAdvertisements = scrapedAdvertisementsGetResult.Value;

        await _processingOrchestrator.ProcessAsync(scrapedAdvertisements.Take(10));

        return Result.Success();
    }
}
