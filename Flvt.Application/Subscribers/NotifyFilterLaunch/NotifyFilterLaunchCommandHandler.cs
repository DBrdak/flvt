using Flvt.Application.Abstractions;
using Flvt.Application.Messaging;
using Flvt.Domain.Filters;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.Subscribers;
using Serilog;

namespace Flvt.Application.Subscribers.NotifyFilterLaunch;

internal sealed class NotifyFilterLaunchCommandHandler : ICommandHandler<NotifyFilterLaunchCommand>
{
    private readonly IFilterRepository _filterRepository;
    private readonly IEmailService _emailService;
    private readonly ISubscriberRepository _subscriberRepository;

    public NotifyFilterLaunchCommandHandler(
        IEmailService emailService,
        IFilterRepository filterRepository,
        ISubscriberRepository subscriberRepository)
    {
        _emailService = emailService;
        _filterRepository = filterRepository;
        _subscriberRepository = subscriberRepository;
    }

    public async Task<Result> Handle(NotifyFilterLaunchCommand request, CancellationToken cancellationToken)
    {
        var filtersGetResult = await _filterRepository.GetManyByIdAsync(request.FiltersIds);

        if (filtersGetResult.IsFailure)
        {
            return filtersGetResult.Error;
        }

        var filters = filtersGetResult.Value.ToList();

        var subscribersEmails = filters.Select(f => f.SubscriberEmail).Distinct();

        var subscribersGetResult = await _subscriberRepository.GetManyByEmailAsync(subscribersEmails);

        if (subscribersGetResult.IsFailure)
        {
            return subscribersGetResult.Error;
        }

        var subscribers = subscribersGetResult.Value.ToList();

        List<SubscriberFilter> subscribersFilters = [];

        foreach (var filter in filters)
        {
            var subscriber = subscribers.FirstOrDefault(s => s.Email.Value == filter.SubscriberEmail);

            if (subscriber is null)
            {
                Log.Error(
                    "Filter have no subscriber, expected subscriber's email: {expectedEmail}, filter ID: {filterId}",
                    filter.Id,
                    filter.SubscriberEmail);

                continue;
            }

            subscribersFilters.Add(new(subscriber, filter));
        }

        var emailTasks = subscribersFilters.Select(sf => 
            _emailService
                .SendFilterLaunchNotificationAsync(sf.Subscriber, sf.Filter));

        var emailResults = await Task.WhenAll(emailTasks);

        return Result.Aggregate(emailResults);
    }
}