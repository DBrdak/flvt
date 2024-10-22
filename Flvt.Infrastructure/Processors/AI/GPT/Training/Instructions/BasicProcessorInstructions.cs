namespace Flvt.Infrastructure.Processors.AI.GPT.Training.Instructions;

internal sealed class BasicProcessorInstructions
{
    private const string generalInstructions =
        "You’re an expert data processor specializing in transforming advertisement objects for real estate listings. Your experience allows you to analyze and standardize data from various sources, creating clear and concise structured outputs that adhere to specific schemas.\r\n\r\nYour task is to analyze the provided advertisement JSON and create a new `ProcessedAdvertisement` object based on the given schema. Here is the original advertisement JSON object you will be working with:";
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
        "Price - This field must sum all costs that a customer will pay for the flat. You would likely search for this information in the Description or Characteristics field. Example scenario: Price = 2500 and Rent = 300, so this field in your response should be 2800";
    private const string depositInstruction =
        "Deposit - if deposit amount is given specify it here. Set the deposit to null if you find no information about it. Note that the Amount and Currency field are not nullable.";
    private const string feeInstruction =
        "Fee - amount that needs to be paid to real estate agency, skip it if it is private offer or no fee is required.";
    private const string facilitiesInstruction =
        "Facilities - provide the facilities that are given in the advertisement. If you can't find any, leave the list empty.";
    private const string availableFromInstruction =
        "AvailableFrom - if provided in the advertisement write the date in the format \"YYYY-MM-DD\", if there is no exact date - provide just a month or \"from now\" but in the native language, if it is not given - skip this field.";
    private const string petsInstruction =
        "Pets - put true if pets are allowed, false if not allowed and null if information is not provided";
    private const string endingInstructions =
        "You should copy other fields as long as there is no error (e.g. missing field etc.). If there is any error, you can fix it if you have enough information to do so (e.g. missing field 'unit' in rooms - you can add it, etc.). Please note that, all currencies must be given in international currency code";

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
        depositInstruction,
        feeInstruction,
        facilitiesInstruction,
        availableFromInstruction,
        petsInstruction,
        endingInstructions,
        "\n"
    };

    public static readonly string CompleteInstruction = string.Join(" ", _allSortedInstructions);
}