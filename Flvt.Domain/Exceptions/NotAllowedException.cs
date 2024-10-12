namespace Flvt.Domain.Exceptions;

public sealed class NotAllowedException : Exception
{
    public NotAllowedException(
        string memberName,
        Type invoker) : base($"{memberName} in {invoker} is not accessible")
    {
        
    }
}