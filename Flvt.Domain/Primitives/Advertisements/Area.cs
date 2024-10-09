namespace Flvt.Domain.Primitives.Advertisements;

public sealed record Area(decimal Value, string Unit)
{
    public override string ToString() => $"{Value} {Unit}";
}