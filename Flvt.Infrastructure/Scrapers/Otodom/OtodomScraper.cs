﻿using Flvt.Domain.Filters;
using Flvt.Infrastructure.Scrapers.Shared;

namespace Flvt.Infrastructure.Scrapers.Otodom;

internal sealed class OtodomScraper : AdvertisementScraper
{
    public OtodomScraper(Filter filter) : base(filter, new OtodomParser())
    { }
}