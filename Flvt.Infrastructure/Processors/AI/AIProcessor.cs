using Flvt.Domain.Advertisements;
using Flvt.Domain.Primitives;
using Flvt.Infrastructure.Processors.AI.GPT;
using Flvt.Infrastructure.Processors.AI.GPT.Utils;
using Flvt.Infrastructure.Processors.AI.Models;
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
        var messages = scrapedAdvertisements.Select(JsonConvert.SerializeObject);

        var replyResult = await _gptClient.MessageAsync(
            AssistantVariant.BasicAdvertisementProcessor,
            messages,
            cancellationToken);

        if (replyResult.IsFailure)
        {
            return replyResult.Error;
        }

        var replies = replyResult.Value;

        var processedAdvertisements = replies
            .Select(JsonConvert.DeserializeObject<ProcessedAdvertisement>)
            .ToList();

        return processedAdvertisements.Any(x => x is null) ?
            AIProcessorErrors.DeserializationError :
            Result.Success(processedAdvertisements);
    }
}