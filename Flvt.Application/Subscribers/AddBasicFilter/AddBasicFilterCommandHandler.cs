using Flvt.Application.Messaging;
using Flvt.Application.Subscribers.Models;
using Flvt.Domain.Primitives.Responses;

namespace Flvt.Application.Subscribers.AddBasicFilter;

internal sealed class AddBasicFilterCommandHandler : ICommandHandler<AddBasicFilterCommand, FilterModel>
{
    public async Task<Result<FilterModel>> Handle(AddBasicFilterCommand request, CancellationToken cancellationToken)
    {
        return null;
    }
}
