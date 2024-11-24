using Flvt.Domain.Extensions;

namespace Flvt.Infrastructure.Scrapers.Shared.Helpers;

internal static class LocationsHelper
{
    public static Dictionary<string, string> OtodomLocations = new()
    {
        { "warszawa", "mazowieckie/warszawa/" },
        { "piaseczno", "mazowieckie/piaseczynski/piaseczno" },
        { "lomianki", "mazowieckie/warszawski-zachodni/lomianki" },
        { "wolomin", "mazowieckie/wolominski/wolomin" },
        { "zabki", "mazowieckie/wolominski/zabki" },
        { "marki", "mazowieckie/wolominski/marki" },
        { "legionowo", "mazowieckie/legionowski/legionowo" },
        { "pruszkow", "mazowieckie/pruszkowski/pruszkow" },
        { "piastow", "mazowieckie/pruszkowski/piastow" },
        { "krakow", "malopolskie/krakow" },
        { "wroclaw", "dolnoslaskie/wroclaw" },
        { "poznan", "wielkopolskie/poznan" },
        { "gdansk", "pomorskie/gdansk" },
        { "lodz", "lodzkie/lodz" },
        { "katowice", "slaskie/katowice" },
        { "gdynia", "pomorskie/gdynia" },
        { "lublin", "lubelskie/lublin" },
        { "bialystok", "podlaskie/bialystok" },
        { "szczecin", "zachodniopomorskie/szczecin" },
        { "torun", "kujawsko--pomorskie/torun" },
    };

    public static Dictionary<string, string> DomiportaLocations = new()
    {
        { "warszawa", "mazowieckie/warszawa" },
        { "piaseczno", "mazowieckie/piaseczno" },
        { "lomianki", "mazowieckie/lomianki" },
        { "wolomin", "mazowieckie/wolomin" },
        { "zabki", "mazowieckie/zabki" },
        { "marki", "mazowieckie/marki" },
        { "legionowo", "mazowieckie/legionowo" },
        { "pruszkow", "mazowieckie/pruszkow" },
        { "piastow", "mazowieckie/piastow" },
        { "krakow", "malopolskie/krakow" },
        { "wroclaw", "dolnoslaskie/wroclaw" },
        { "poznan", "wielkopolskie/poznan" },
        { "gdansk", "pomorskie/gdansk" },
        { "lodz", "lodzkie/lodz" },
        { "katowice", "slaskie/katowice" },
        { "gdynia", "pomorskie/gdynia" },
        { "lublin", "lubelskie/lublin" },
        { "bialystok", "podlaskie/bialystok" },
        { "szczecin", "zachodniopomorskie/szczecin" },
        { "torun", "kujawsko--pomorskie/torun" },
    };

    public static string? OtodomLocation(this ScrapingFilter filter) =>
        OtodomLocations.GetValueOrDefault(filter.City.ToLower().ReplacePolishCharacters());

    public static string? DomiportaLocation(this ScrapingFilter filter) =>
        DomiportaLocations.GetValueOrDefault(filter.City.ToLower().ReplacePolishCharacters());
}