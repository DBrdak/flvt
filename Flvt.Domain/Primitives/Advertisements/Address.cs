namespace Flvt.Domain.Primitives.Advertisements;

public sealed record Address(
    string Country,
    string State,
    string Region,
    string City,
    string District,
    string Street,
    string HouseNumber);