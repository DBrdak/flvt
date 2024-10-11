using Flvt.Domain.Extensions;
using Flvt.Domain.Primitives.Subscribers.Filters;

namespace Flvt.Infrastructure.Scrapers.Otodom;

internal static class OtodomLocationsDictionary
{
    public static Dictionary<string, string> Locations = new()
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
        { "torun", "kujawsko-pomorskie/torun" },
    };

    public static string? OtodomLocation(this Filter filter) => 
        Locations.GetValueOrDefault(filter.Location.City.ToLower().ReplacePolishCharacters());
}