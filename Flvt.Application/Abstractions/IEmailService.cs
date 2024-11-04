using Flvt.Domain.Filters;
using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.Subscribers;

namespace Flvt.Application.Abstractions;

public interface IEmailService
{
    Task<Result> SendVerificationEmailAsync(Subscriber subscriber);

    Task<Result> SendResetPasswordEmailAsync(Subscriber subscriber);

    Task<Result> SendFilterLaunchNotificationAsync(Subscriber subscriber, Filter filter);
}