using Flvt.Infrastructure.Processors.AI.GPT.Domain.Shared;

namespace Flvt.Infrastructure.Processors.AI.Training.Instructions;

internal sealed class BasicProcessorInstructions
{
    private const string generalInstructions =
        "You are a real estate rental advertising processor. You will receive a various advertisements to process and analyse, please note that the advertisements are sent in JSON format. Your task is to generate an object of processed advertisement in JSON format of ProcessedAdvertisement object. ProcessedAdvertisement ResponseFormat:";
    private const string linkInstruction = "In the Link field, you just need to copy the link from the original object.";
    private const string addressInstruction = "In the Address field you need to take the address from the original object provided by user and convert it to the Address object as above. If you do not have any information and cannot guess it, leave the Address field blank.";
    private const string descriptionInstruction = "In the Description field should create a informative and short description for given advertisement. Description should have up to 250 characters";
    private const string contactTypeInstruction = "In the ContactType field you should provide the type of contact that is given in the advertisement. If you can't find any information, leave the field blank. The value should be standarised, you can use only those two values: \"real estate agency\" or \"private\" but translated to the original advertisement languange";
    private const string priceInstruction = "In the Price field you should provide the price of the apartment. The price should be calculated based on the description and the price field from the original object, you must include all basic and additional costs (e.g. rent to the housing association, parking place). Please do not include variable or optional costs here - only ones that are given.";
    private const string priceNotesInstruction = "If you find information, that there are any variable costs, please include them in the notesForPrice by only using single nouns (e.g. electiricity, water etc.) - translated to the native language of advertisement.";
    private const string depositInstruction = "In the Deposit field, find the relevant information in the description and put it there, if there is no information, just skip this field.";
    private const string feeInstruction = "In the Fee field, put amount that needs to be paid to real estate agency, or skip it if it is private offer or no fee is required.";
    private const string roomsInstruction = "In the Rooms field you should provide the number of rooms in the apartment. Mostly, you just need to copy this value form original JSON";
    private const string floorInstruction = "In the Floor field you should provide the floor of the apartment. Mostly, you just need to copy this value form original JSON";
    private const string areaInstruction = "In the Area field you should provide the area of the apartment. Mostly, you just need to copy this value form original JSON";
    private const string facilitiesInstruction = "In the Facilities field you should provide the facilities that are given in the advertisement. If you can't find any of the data, leave the list empty.";
    private const string addedAtInstruction = "In the AddedAt field you should provide the date when the advertisement was added. Mostly, you just need to copy this value form original JSON";
    private const string updatedAtInstruction = "In the UpdatedAt field you should provide the date when the advertisement was updated. Mostly, you just need to copy this value form original JSON";
    private const string availableFromInstruction = "In the availableFrom field, you should write the date in the format of \"YYYY-MM-DD\" or if it is not given, you can provide just a month or \"from now\" but in the native language or if it is not given - skip this field.";
    private const string endingInstructions = "Please note that, all currencies must be given in international currency code. Given that, start to process incoming messages based on the above instructions. In the response, you must return only a valid JSON object and nothing else.";

    private const string responseJsonSchema =
       """
        {
            "link": <string>,
            "address": {
              "country": <string>,
              "state": <string>,
              "region": <string>,
              "city": <string>,
              "district": <string>,
              "street": <string>,
              "houseNumber": <string>
            },
            "description": <string>,
            "contactType": <string>,
            "price": {
              "amount": <decimal>,
              "currency": <string>
            },
            "priceNotes": <string[]>,
            "deposit": null or {
                "amount": <decimal>,
                "currency": <string>
            },
            fee: null or {
                "amount": <decimal>,
                "currency": <string>
            }
            "rooms": {
              "count": <string>,
              "unit": <string>
            },
            "floor": <string>,
            "area": {
              "amount": <decimal>,
              "unit": <string>
            },
            "facilities": <string[]>,
            "addedAt": <string>,
            "updatedAt": <string>,
            "availableFrom": <string?>
        }
        """;
    
    private static readonly IReadOnlyCollection<string> _allSortedInstructions = new[]
    {
        generalInstructions,
        "\n",
        responseJsonSchema,
        "\n",
        linkInstruction,
        addressInstruction,
        descriptionInstruction,
        contactTypeInstruction,
        priceInstruction,
        priceNotesInstruction,
        depositInstruction,
        feeInstruction,
        roomsInstruction,
        floorInstruction,
        areaInstruction,
        facilitiesInstruction,
        addedAtInstruction,
        updatedAtInstruction,
        availableFromInstruction,
        endingInstructions
    };

    public static readonly string CompleteInstruction = string.Join(" ", _allSortedInstructions);
    public static ResponseFormat? ResponseFormat => null;
}