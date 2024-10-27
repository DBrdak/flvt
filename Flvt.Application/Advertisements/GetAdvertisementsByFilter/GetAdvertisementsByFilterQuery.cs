using Flvt.Application.Advertisements.Models;
using Flvt.Application.Messaging;
using Flvt.Domain.Primitives.Responses;

namespace Flvt.Application.Advertisements.GetAdvertisementsByFilter;

public sealed record GetAdvertisementsByFilterQuery(
    string? FilterId,
    string? SubscriberEmail,
    int? PageSize,
    int? Page,
    string? SortOrder,
    string? SortBy) : IQuery<Page<ProcessedAdvertisementModel>>;
