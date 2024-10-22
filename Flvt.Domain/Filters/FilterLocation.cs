using Flvt.Domain.Filters.Erros;
using Flvt.Domain.Primitives.Responses;

namespace Flvt.Domain.Filters;

public sealed record FilterLocation
{
    public string City { get; init; }
    /// <summary>
    /// Warsaw agglomeration only (2024-10-13)
    /// </summary>
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
            //new("krakow"),
            //new("wroclaw"),
            //new("poznan"),
            //new("gdansk"),
            //new("szczecin"),
            //new("lodz"),
            //new("katowice"),
            //new("gdynia"),
            //new("lublin"),
            //new("bialystok"),
            //new("szczecin"),
            //new("torun")
        ];

    private FilterLocation(string city) => City = city;

    internal static Result<FilterLocation> Create(string city) =>
        SupportedCities.FirstOrDefault(location => location.City == city.ToLower()) switch
        {
            { } location => location,
            _ => FilterErrors.LocationNotSupported
        };
    public override string ToString() => $"{City}";
}