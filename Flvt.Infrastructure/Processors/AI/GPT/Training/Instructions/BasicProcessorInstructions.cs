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
              "city": <string>,
              "district": <string>,
              "street": <string>,
              "houseNumber": <string>
            },
            "geolocation": null or {
                "latitude": <string>,
                "longitude": <string>
            },
            "description": <string>,
            "isPrivate": <boolean>,
            "price": {
              "amount": <decimal>,
              "currency": {
                "code": <string>,
              }
            },
            "deposit": <decimal?>,
            "fee": <decimal?>,
            "roomsCount": <int>,
            "area": {
              "amount": <decimal>,
              "unit": <string>
            },
            "addedAt": <string>,
            "updatedAt": <string>,
            "availableFrom": <string?>,
            "pets": <boolean?>
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
    private const string isPrivateInstruction =
        "IsPrivate - find the contact type from the advertisement, set this value to true if it is private offer, or false if this is not private offer (e.g. real estate agency).";
    private const string roomsInstruction =
        "RoomsCount - count of the rooms in the flat, this value must be greater than 0";
    private const string priceInstruction =
        "Price - This field must sum all costs that a customer will pay for the flat. You would likely search for this information in the Description or Characteristics field. Example scenario: Price = 2500 and Administrative Rent = 300, so this field in your response should be 2800";
    private const string depositInstruction =
        "Deposit - if deposit amount is given specify it here. Set the deposit to null if you find no information about it.";
    private const string feeInstruction =
        "Fee - amount that needs to be paid to real estate agency, skip it if it is private offer or no fee is required.";
    private const string availableFromInstruction =
        "AvailableFrom - if provided in the advertisement write the date in the format \"YYYY-MM-DD\", if there is no exact date - provide just a month or \"from now\" but in the native language, if it is not given - skip this field.";
    private const string petsInstruction =
        "Pets - put true if pets are allowed, false if not allowed and null if information is not provided";
    private const string endingInstructions =
        "You should copy other fields. " +
        "If there is any error, you can fix it if you have enough information to do so." +
        "Please note that, all currencies must be given in international currency code. " +
        "All text data should be returned in english.";

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
        isPrivateInstruction,
        roomsInstruction,
        priceInstruction,
        depositInstruction,
        feeInstruction,
        availableFromInstruction,
        petsInstruction,
        endingInstructions,
        "\n"
    };

    public static readonly string CompleteInstruction = string.Join(" ", _allSortedInstructions);
}