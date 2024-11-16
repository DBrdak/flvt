namespace Flvt.Domain.Extensions;

public static class ListExtensions
{
    public static void Remove<T>(this List<T> list, T item, out bool isRemoved)
    {
        isRemoved = list.Remove(item);
    }
}