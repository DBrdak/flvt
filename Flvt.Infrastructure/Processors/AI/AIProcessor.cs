using Flvt.Domain.Advertisements;
using Flvt.Domain.Primitives;
using Flvt.Infrastructure.Processors.AI.GPT;
using Flvt.Infrastructure.Processors.AI.GPT.Messages;
using Flvt.Infrastructure.Processors.AI.GPT.Utils;
using Newtonsoft.Json;

namespace Flvt.Infrastructure.Processors.AI;

internal sealed class AIProcessor
{
    private readonly GPTClient _gptClient;

    public AIProcessor(GPTClient gptClient)
    {
        _gptClient = gptClient;
    }

    public async Task<Result<List<ProcessedAdvertisement>>> ProcessBasicAdvertisementAsync(
        IEnumerable<ScrapedAdvertisement> scrapedAdvertisements,
        CancellationToken cancellationToken)
    {
        var messages = scrapedAdvertisements.Select(GPTMessageFactory.CreateBasicMessage).ToList()[..9];
        List<Task<string?>> replyTasks = [];

        foreach (var message in messages)
        {
            replyTasks.Add(_gptClient.CreateCompletionAsync(
                message,
                GPTModel.Mini4o));
        }

        var replyResults = await Task.WhenAll(replyTasks);

        var replies = replyResults.Where(r => r is not null);

        var processedAdvertisements = replies.Select(JsonConvert.DeserializeObject<ProcessedAdvertisement?>)
            .Where(ad => ad is not null)
            .ToList();

        return processedAdvertisements;
    }
}