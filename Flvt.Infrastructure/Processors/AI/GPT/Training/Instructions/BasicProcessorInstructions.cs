﻿namespace Flvt.Infrastructure.Processors.AI.GPT.Training.Instructions;

internal sealed class BasicProcessorInstructions
{
    private const string generalInstructions =
        "The JSON provided by the user is an advertisement object scraped from a website. Based on the below instructions, provided advertisement and given JSON schema, you must analyse it and create new ProcessedAdvertisement object, following this schema:";
    private const string responseJsonSchema =
        """
        {
            "link": <string>,
            "address": {
              "country": <string>,
              "province": <string>,
              "region": <string>,
              "city": <string>,
              "district": <string>,
              "subdistrict": <string>,
              "street": <string>,
              "houseNumber": <string>
            },
            "geolocation": null or {
                "latitude": <string>,
                "longitude": <string>
            },
            "description": <string>,
            "contactType": <string>,
            "price": {
              "amount": <decimal>,
              "currency": {
                "code": <string>,
              }
            },
            "priceNotes": <string[]>,
            "deposit": null or {
                "amount": <decimal>,
                "currency": {
                    "code": <string>,
                }
            },
            fee: null or {
                "amount": <decimal>,
                "currency": {
                    "code": <string>,
                }
            }
            "rooms": {
              "count": <string>,
              "unit": <string>
            },
            "floor": {
                "specific": <int>,
                "total": <int>
            },
            "area": {
              "amount": <decimal>,
              "unit": <string>
            },
            "facilities": <string[]>,
            "addedAt": <string>,
            "updatedAt": <string>,
            "availableFrom": <string?>,
            "pets": <boolean?>,
            "photos": <string[]>
        }
        """;
    private const string linkInstruction =
        "Link - copy this value form original object";
    private const string addressInstruction =
        "Address - standarize the address from original object to given Address object.";
    private const string geolocationInstruction =
        "Geolocation - Provide if given in the original object and skip if not";
    private const string descriptionInstruction =
        "Description - create a informative and short description for given advertisement, up to 200 characters.";
    private const string contactTypeInstruction =
        "ContactType - find the contact type from the previous advertisement and standarize it, to \"real estate agency\" or \"private\" but translated to the original advertisement languange. If field is empty in previous object, find information in description and update.";
    private const string floorInstruction =
        "Floor - specific is the floor on which the flat is placed and total is the total amout of floors in the builiding. If floor is specified as groundfloor, set 0 then";
    private const string priceInstruction =
        "Price - It should include all costs that the tenant must pay monthly, most of the time it won't be only the flat price. Gather all data about monthly constant costs and put it to this field. You should include constant costs (e.g. rent [czynsz administracyjny], price). You shouldn't include variable costs (e.g. electricity, water) and optional costs (e.g. parking, storage cell) unless they are required. Example scenario: Price = 2500 and Rent = 300, so this field in your response should be 2800";
    private const string priceNotesInstruction =
        "PriceNotes - if you find information about variable or extra costs, add it here. You can use only nouns (e.g. electricity, water, etc.) translated to native language. DO NOT include costs included in the other fields like Price, Fee or Deposit (e.g. rent to the housing association - czynsz SHOULD NOT BE INCLUDED).";
    private const string depositInstruction =
        "Deposit - if deposit amount is given specify it here, set the deposit to null (whole field) if you find no information about it.";
    private const string feeInstruction =
        "Fee - amount that needs to be paid to real estate agency, skip it if it is private offer or no fee is required.";
    private const string facilitiesInstruction =
        "Facilities - provide the facilities that are given in the advertisement. If you can't find any, leave the list empty.";
    private const string availableFromInstruction =
        "AvailableFrom - if provided in the advertisement write the date in the format \"YYYY-MM-DD\", if there is no exact date - provide just a month or \"from now\" but in the native language, if it is not given - skip this field.";
    private const string petsInstruction =
        "Pets - put true if pets are allowed, false if not allowed and null if information is not provided";
    private const string photosInstruction =
        "Photos - always should be copied.";
    private const string endingInstructions =
        "You should copy other fields as long as there is no error (e.g. missing field etc.). If there is any error, you can fix it if you have enough information to do so (e.g. missing field 'unit' in rooms - you can add it, etc.). Please note that, all currencies must be given in international currency code (you can adjust all curencies - even copied ones). Given that, start to process incoming messages based on the above instructions. In the response, you must return only a valid JSON object and nothing else.";

    private static readonly IReadOnlyCollection<string> _allSortedInstructions = new[]
    {
        generalInstructions,
        "\n",
        responseJsonSchema,
        "\n",
        linkInstruction,
        addressInstruction,
        geolocationInstruction,
        descriptionInstruction,
        contactTypeInstruction,
        floorInstruction,
        priceInstruction,
        priceNotesInstruction,
        depositInstruction,
        feeInstruction,
        facilitiesInstruction,
        availableFromInstruction,
        petsInstruction,
        photosInstruction,
        endingInstructions,
        "\n"
    };

    public static readonly string CompleteInstruction = string.Join(" ", _allSortedInstructions);
}