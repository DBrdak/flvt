using Flvt.Domain.Primitives.Responses;

namespace Flvt.Domain.Primitives.Filters.Erros;

internal sealed class FilterErrors
{
    public static Error LocationNotSupported => new ("Given location not supported");
    public static Error InvalidName => new ("Invalid name specified");
    public static Error InvalidRoomsCount => new("Rooms count must be between 1 and 6");
    public static Error InvalidArea => new("Area must be between 1 and 1 000");
    public static Error InvalidPrice => new("Price must be between 1 and 100 000 000");
}