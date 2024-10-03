namespace Flvt.Domain.Primitives;

public sealed record Error
{
    public static Error None { get; } = new Error(string.Empty);
    public static Error NullValue { get; } = new Error("Null value provided");

    public string Message { get; }

    internal Error(string message)
    {
        Message = message;
    }

    public override string ToString() => Message;
}