using Flvt.Application.Messaging;
using Flvt.Domain.Filters;
using Flvt.Domain.Primitives.Responses;

namespace Flvt.Application.Advertisements.Follow;

internal sealed class FollowCommandHandler : ICommandHandler<FollowCommand>
{
    private readonly IFilterRepository _filterRepository;

    public FollowCommandHandler(IFilterRepository filterRepository)
    {
        _filterRepository = filterRepository;
    }

    public async Task<Result> Handle(FollowCommand request, CancellationToken cancellationToken)
    {
        var filterGetResult = await _filterRepository.GetByIdAsync(request.FilterId);

        if (filterGetResult.IsFailure)
        {
            return filterGetResult.Error;
        }

        var filter = filterGetResult.Value;

        filter.Follow(request.AdvertisementLink);

        return await _filterRepository.UpdateAsync(filter);
    }
}
