using Flvt.Domain.Advertisements;
using Flvt.Infrastructure.Processors.AI.Models;

namespace Flvt.Infrastructure.Processors.AI;

internal static class PromptFactory
{
    public static Prompt CreateDefaultAdvertisementProcessingPrompt(ScrapedAdvertisement advertisement) => 
        Prompt.ForceCreate($$"""
                      Read the below apartament advertisement description and all needed data about it given in JSON format. Then analyse it and write response in the following (JSON) format:
                      {{SemiProcessedAdvertisement.JsonRepresentation}}
                      Please note that the shortDescription should have up to 250 characters and be as simple as possible, facilities up to 10 elements. If you can't find any of the data, leave the list empty. In the availableFrom field, you should write the date in the format of "YYYY-MM-DD" or if it is not given, you can provide just a month or "from now" but in the native language or if it is not given - skip this field.The price should be calculated based on the description, you must include all basic and additional costs. If you find information, that there are any variable costs, please include them in the notesForPrice by only using single nouns (e.g. electiricity, water, estate agency fee, etc.). In the deposit field, find the relevant information in the description and put it there, if there is no information, just skip this field. In the Link field, you just need to copy the link from the original object. Do not include currency in the price and deposit.You must respond in the same language as the ad is written in and your response must be only a object that I gave you before in JSON.
                      Advertisement:
                      {
                        "link": {{advertisement.Link}},
                        "description": {{advertisement.Description}},
                        "basicPrice": {{advertisement.Price}},
                        "rooms": {{advertisement.Rooms}},
                        "floor": {{advertisement.Floor}},
                        "area": {{advertisement.Area}}
                      }
                      """);
}