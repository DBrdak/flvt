using Flvt.Application.Messaging;
using Flvt.Domain.Primitives.Responses;

namespace Flvt.Application.Advertisements.MarkAsSeen;

internal sealed class MarkAsSeenCommandHandler : ICommandHandler<MarkAsSeenCommand>
{
    public async Task<Result> Handle(MarkAsSeenCommand request, CancellationToken cancellationToken)
    {
        return null;
    }
}
