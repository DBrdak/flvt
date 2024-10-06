using Flvt.Domain.Extensions;
using Flvt.Domain.Subscribers;

namespace Flvt.Infrastructure.Scrapers.Otodom;

internal static class OtodomLocationsDictionary
{
    public static Dictionary<string, string> Locations = new()
    {
        { "warszawa", "mazowieckie/warszawa/warszawa/warszawa" },
        { "krakow", "malopolskie/krakow/krakow/krakow" },
        { "wroclaw", "dolnoslaskie/wroclaw/wroclaw/wroclaw" },
        { "poznan", "wielkopolskie/poznan/poznan/poznan" },
        { "gdansk", "pomorskie/gdansk/gdansk/gdansk" },
        { "szczecin", "zachodniopomorskie/szczecin/szczecin/szczecin" },
        { "lodz", "lodzkie/lodz/lodz/lodz" },
        { "katowice", "slaskie/katowice/katowice/katowice" },
        { "gliwice", "slaskie/gliwice/gliwice/gliwice" },
        { "zabrze", "slaskie/zabrze/zabrze/zabrze" },
        { "chorzow", "slaskie/chorzow/chorzow/chorzow" },
        { "tychy", "slaskie/tychy/tychy/tychy" },
        { "gdynia", "gdynia" }
    };

    public static string? OtodomLocation(this Filter filter) => 
        Locations.GetValueOrDefault(filter.Location?.ToLower().ReplacePolishCharacters() ?? string.Empty);
}