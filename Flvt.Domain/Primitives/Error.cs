namespace Flvt.Domain.Primitives;

public sealed record Error
{
    public static Error NotFound<T>() => new ($"{typeof(T).Name} not found");
    public static Error None { get; } = new Error(string.Empty);
    public static Error NullValue { get; } = new Error("Null value provided");

    public string Message { get; }

    public Error(string message)
    {
        Message = message;
    }

    public override string ToString() => Message;
}