namespace Flvt.Domain.Primitives.Advertisements;

public sealed record Coordinates(string Latitude, string Longitude)
{
    public override string ToString() => $"({Latitude}, {Longitude})";
}