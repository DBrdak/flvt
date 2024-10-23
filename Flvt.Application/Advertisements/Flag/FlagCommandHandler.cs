using Flvt.Application.Messaging;
using Flvt.Domain.Primitives.Responses;

namespace Flvt.Application.Advertisements.Flag;

internal sealed class FlagCommandHandler : ICommandHandler<FlagCommand>
{
    public async Task<Result> Handle(FlagCommand request, CancellationToken cancellationToken)
    {
        return null;
    }
}
