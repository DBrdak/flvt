using Flvt.Application.Advertisements.Models;
using Flvt.Application.Messaging;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.ProcessedAdvertisements;
using Serilog;

namespace Flvt.Application.Advertisements.GetAllAdvertisements;

internal sealed class GetAllAdvertisementsQueryHandler :
    IQueryHandler<GetAllAdvertisementsQuery, IEnumerable<ProcessedAdvertisementModel>>
{
    private readonly IProcessedAdvertisementRepository _processedAdvertisementRepository;

    public GetAllAdvertisementsQueryHandler(
        IProcessedAdvertisementRepository processedAdvertisementRepository)
    {
        _processedAdvertisementRepository = processedAdvertisementRepository;
    }

    public async Task<Result<IEnumerable<ProcessedAdvertisementModel>>> Handle(
        GetAllAdvertisementsQuery request,
        CancellationToken cancellationToken)
    {
        var processedAdvertisementsGetResult = await _processedAdvertisementRepository.GetAllAsync();

        if (processedAdvertisementsGetResult.IsFailure)
        {
            return processedAdvertisementsGetResult.Error;
        }

        var result = Result.Create(
            processedAdvertisementsGetResult.Value.Select(ProcessedAdvertisementModel.FromDomainModel));

        return result;
    }
}
