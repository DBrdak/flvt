namespace Flvt.Application.Abstractions;

public interface ILinkScrapingOrchestrator
{
    Task<IEnumerable<string>> ScrapeAsync(string service, string city, bool onlyNew = true);
}