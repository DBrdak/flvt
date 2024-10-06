using Flvt.Domain.Primitives;

namespace Flvt.Domain.Advertisements.Errors;

internal static class ScrapedAdvertisementErrors
{
    public static Error LocationNotFound => new ("Location not found in the advertisement");
    public static Error DescriptionNotFound => new ("Description not found in the advertisement");
    public static Error ContactTypeNotFound => new ("ContactType not found in the advertisement");
    public static Error PriceNotFound => new ("Price not found in the advertisement");
    public static Error DepositNotFound => new ("Deposit not found in the advertisement");
    public static Error CurrencyNotFound => new ("Currency not found in the advertisement");
    public static Error RoomsNotFound => new ("Rooms not found in the advertisement");
    public static Error AreaNotFound => new ("Area not found in the advertisement");
}