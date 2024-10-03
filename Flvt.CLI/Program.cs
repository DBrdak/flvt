using Flvt.Domain.Advertisements;
using Flvt.Scraper;
using Newtonsoft.Json;

namespace Flvt.CLI;

internal class Program
{
        
    static async Task Main(string[] args)
    {
        var a = new ScrapingOrchestrator();
        var filter = new Filter()
            .InLocation("warszawa")
            .ToArea(45)
            .FromArea(30)
            .ToPrice(2800)
            .FromRooms(2)
            .ToRooms(4)
            .Build();

        var b = await a.ScrapeAsync(filter);

        Console.WriteLine(JsonConvert.SerializeObject(b));
    }
}