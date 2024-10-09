﻿using Flvt.Domain.Primitives.Responses;
using Flvt.Domain.Primitives.Subscribers.Filters.Erros;

namespace Flvt.Domain.Primitives.Subscribers.Filters;

public sealed record FilterLocation
{
    public string City { get; init; }
    public static readonly IReadOnlyCollection<FilterLocation> SupportedCities =
        [
            new("warszawa"),
            new("piaseczno"),
            new("lomianki"),
            new("wolomin"),
            new("zabki"),
            new("marki"),
            new("legionowo"),
            new("pruszkow"),
            new("piastow"),
            new("krakow"),
            new("wroclaw"),
            new("poznan"),
            new("gdansk"),
            new("szczecin"),
            new("lodz"),
            new("katowice"),
            new("gliwice"),
            new("zabrze"),
            new("chorzow"),
            new("tychy"),
            new("gdynia")
        ];

    private FilterLocation(string city) => City = city;

    internal static Result<FilterLocation> Create(string city) =>
        SupportedCities.FirstOrDefault(location => location.City == city.ToLower()) switch
        {
            { } location => location,
            _ => FilterErrors.LocationNotSupported
        };
}