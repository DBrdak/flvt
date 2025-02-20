﻿using Flvt.Domain.ScrapedAdvertisements;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Batches;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Chat.Completions;
using Flvt.Infrastructure.Processors.AI.GPT.Training.Instructions;
using Flvt.Infrastructure.Processors.AI.GPT.Utils;
using Newtonsoft.Json;

namespace Flvt.Infrastructure.Processors.AI.GPT.Messages;

internal sealed class GPTMessageFactory
{
    private const string userRole = "user";
    private const string systemRole = "system";

    public static IEnumerable<Message> CreateBasicMessages(ScrapedAdvertisement advertisement) =>
        [
            new(systemRole, BasicProcessorInstructions.CompleteInstruction),
            new (userRole, JsonConvert.SerializeObject(advertisement))
        ];

    public static BatchMessageLine CreateBasicBatchLine(
        ScrapedAdvertisement scrapedAdvertisement,
        string method = "POST",
        string url = GPTPaths.CreateCompletion,
        string model = GPTModel.Mini4oPrimitive) =>
        new(
            scrapedAdvertisement.Link,
            new CompletionCreateRequest(
                Messages: CreateBasicMessages(scrapedAdvertisement).ToList(),
                Model: GPTModel.Mini4o.Value,
                Temperature: GPTFineTuneDefaults.LowTemperature,
                TopP: GPTFineTuneDefaults.MaxTopP,
                ResponseFormat: GPTResponseFormats.JsonObject,
                Store: true));
}