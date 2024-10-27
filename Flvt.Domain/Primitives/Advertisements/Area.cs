namespace Flvt.Domain.Primitives.Advertisements;

public sealed record Area(decimal Amount, string Unit)
{
    public override string ToString() => $"{Amount} {Unit}";
}