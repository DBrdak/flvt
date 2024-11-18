namespace Flvt.Infrastructure.Scrapers.Shared.Helpers;

internal sealed class ScraperHelper
{
    public string Name { get; init; }
    public string Value { get; init; }

    public ScraperHelper(string name, string value)
    {
        Name = name;
        Value = value;
    }
}