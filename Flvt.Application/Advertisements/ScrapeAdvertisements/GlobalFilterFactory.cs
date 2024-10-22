﻿using Flvt.Domain.Filters;

namespace Flvt.Application.Advertisements.ScrapeAdvertisements;

internal sealed class GlobalFilterFactory
{
    public static List<Filter> CreateFiltersForAllLocations() =>
        FilterLocation
            .SupportedCities
            .Select(Filter.CreateForInternalScan)
            .ToList();
}