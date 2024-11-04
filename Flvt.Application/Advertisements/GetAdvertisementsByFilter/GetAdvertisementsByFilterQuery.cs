using Flvt.Application.Messaging;

namespace Flvt.Application.Advertisements.GetAdvertisementsByFilter;

public sealed record GetAdvertisementsByFilterQuery(
    string? FilterId,
    string? SubscriberEmail) : IQuery<string>;
