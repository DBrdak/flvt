namespace Flvt.Domain.Primitives.Advertisements;

public sealed record Address(
    string? Country,
    string? City,
    string? District,
    string? Street,
    string? HouseNumber);