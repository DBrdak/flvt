namespace Flvt.Domain.Primitives;

public sealed record RoomsCount(int Value, string Unit)
{
    public override string ToString() => $"{Value} {Unit}";
}