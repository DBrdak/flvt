namespace Flvt.Domain.Primitives.Advertisements;

public sealed record RoomsCount(int Value, string Unit)
{
    public override string ToString() => $"{Value} {Unit}";
}