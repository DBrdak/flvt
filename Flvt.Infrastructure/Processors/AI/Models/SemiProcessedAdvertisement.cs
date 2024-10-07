namespace Flvt.Infrastructure.Processors.AI.Models;

internal sealed record SemiProcessedAdvertisement(
    string Link,
    string ShortDescription,
    decimal Price,
    string NotesForPrice,
    decimal? Deposit,
    string AvailableFrom,
    string[] Facilities)
{
    public static string JsonRepresentation =>
        """
        {
          "link": <string>,
          "shortDescription": <string>,
          "price": <decimal>,
          "notesForPrice": <string>,
          "deposit": <decimal?>,
          "availableFrom": <string?>,
          "facilities": <string[]>
        }
        """;
}