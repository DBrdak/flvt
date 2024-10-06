using Flvt.Application.ProcessAdvertisements;
using Flvt.Domain.Subscribers;
using Newtonsoft.Json;

namespace Flvt.CLI;

internal class Program
{
        
    static async Task Main(string[] args)
    {
        var filter = new Filter()
            .InLocation("warszawa")
            .ToArea(45)
            .FromArea(30)
            .ToPrice(2800)
            .FromRooms(2)
            .ToRooms(4)
            .Build();
        var cmd = new ProcessAdvertisementsCommand(
            filter.Location,
            filter.MinPrice,
            filter.MaxPrice,
            filter.MinRooms,
            filter.MaxRooms,
            filter.MinArea,
            filter.MaxArea);


    }
}