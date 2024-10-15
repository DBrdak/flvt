using Flvt.Application.Messaging;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.Subscribers;

namespace Flvt.Application.Subscribers.AddBasicFilter;

//TODO Implement
internal sealed class AddBasicFilterCommandHandler : ICommandHandler<AddBasicFilterCommand>
{
    private readonly ISubscriberRepository _subscriberRepository;

    public AddBasicFilterCommandHandler(ISubscriberRepository subscriberRepository)
    {
        _subscriberRepository = subscriberRepository;
    }

    public async Task<Result> Handle(AddBasicFilterCommand request, CancellationToken cancellationToken)
    {
        var subscriberResult = await _subscriberRepository.GetByEmailAsync(request.Email);

        if (subscriberResult.IsFailure)
        {
            return subscriberResult.Error;
        }

        var subscriber = subscriberResult.Value;

        return null;
    }
}
