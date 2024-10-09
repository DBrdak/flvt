using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Chat.Completions;
using Flvt.Infrastructure.Processors.AI.GPT.Messages.Instructions;
using Newtonsoft.Json;

namespace Flvt.Infrastructure.Processors.AI.GPT.Messages;

internal sealed class GPTMessageFactory
{
    private const string userRole = "user";

    public static Message CreateBasicMessage(ScrapedAdvertisement advertisement) =>
        new(
            userRole,
            string.Concat(
                JsonConvert.SerializeObject(advertisement),
                BasicProcessorInstructions.CompleteInstruction)
        );
}