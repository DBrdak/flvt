namespace Flvt.Domain.Primitives.Advertisements;

public sealed record Address(
    string? Country,
    string? Province,
    string? Region,
    string? City,
    string? District,
    string? Subdistrict,
    string? Street,
    string? HouseNumber);