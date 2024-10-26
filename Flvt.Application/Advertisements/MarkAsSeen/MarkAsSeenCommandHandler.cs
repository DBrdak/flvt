using Flvt.Application.Messaging;
using Flvt.Domain.Filters;
using Flvt.Domain.Primitives.Responses;

namespace Flvt.Application.Advertisements.MarkAsSeen;

internal sealed class MarkAsSeenCommandHandler : ICommandHandler<MarkAsSeenCommand>
{
    private readonly IFilterRepository _filterRepository;

    public MarkAsSeenCommandHandler(IFilterRepository filterRepository)
    {
        _filterRepository = filterRepository;
    }

    public async Task<Result> Handle(MarkAsSeenCommand request, CancellationToken cancellationToken)
    {
        var filterGetResult = await _filterRepository.GetByIdAsync(request.FilterId);

        if (filterGetResult.IsFailure)
        {
            return filterGetResult.Error;
        }

        var filter = filterGetResult.Value;

        filter.MarkAsSeen(request.AdvertisementLink);

        return await _filterRepository.UpdateAsync(filter);
    }
}
