using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Domain.Filters;
using Flvt.Domain.Primitives.Responses;

namespace Flvt.Application.Advertisements.GetPreviewAdvertisements;

internal sealed class GetPreviewAdvertisementsQueryHandler : IQueryHandler<GetPreviewAdvertisementsQuery, string>
{
    private readonly IFileService _fileService;
    private readonly IFilterRepository _filterRepository;

    public GetPreviewAdvertisementsQueryHandler(IFileService fileService, IFilterRepository filterRepository)
    {
        _fileService = fileService;
        _filterRepository = filterRepository;
    }

    public async Task<Result<string>> Handle(GetPreviewAdvertisementsQuery request, CancellationToken cancellationToken)
    {
        var filterGetResult = await _filterRepository.GetByIdAsync("preview");

        if (filterGetResult.IsFailure)
        {
            return filterGetResult.Error;
        }

        var filter = filterGetResult.Value;

        return await _fileService.GetAdvertisementsUrlAsync(filter);
    }
}
