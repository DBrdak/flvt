namespace Flvt.Domain.Primitives;

public sealed record Area(decimal Value, string Unit)
{
    public override string ToString() => $"{Value} {Unit}";
}