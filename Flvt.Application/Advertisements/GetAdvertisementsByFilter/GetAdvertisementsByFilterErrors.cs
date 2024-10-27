using Flvt.Domain.Primitives.Responses;

namespace Flvt.Application.Advertisements.GetAdvertisementsByFilter;

internal sealed class GetAdvertisementsByFilterErrors
{
    public static readonly Error InvalidRequest =
        new("Invalid request, one or more of required parameters missing: Page / FilterId / SubscriberEmail");
}