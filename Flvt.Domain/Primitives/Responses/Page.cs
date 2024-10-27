namespace Flvt.Domain.Primitives.Responses;

public sealed class Page<T>
{
    private readonly List<T> _items;
    public IReadOnlyCollection<T> Items => _items;
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }

    private Page(List<T> items, int pageNumber, int pageSize, int totalCount, int totalPages)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = totalPages;
        _items = items;
    }

    public static Page<T> Create(IEnumerable<T> items, int pageSize, int pageNumber)
    {
        var itemsList = items.ToList();
        var itemsInPage = itemsList
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToList();

        return new Page<T>(
            itemsInPage,
            pageNumber,
            pageSize,
            itemsList.Count,
            (int)Math.Ceiling((decimal)itemsList.Count / pageSize));
    }

    public static Page<T> CreateSorted(
        IEnumerable<T> items,
        int pageNumber,
        int pageSize,
        Func<T, object> sortKey,
        SortOrder order)
    {
        var itemsList = items.ToList();
        var sortedItems = order switch
        {
            SortOrder.Ascending => itemsList.OrderBy(sortKey),
            SortOrder.Descending => itemsList.OrderByDescending(sortKey),
            _ => throw new ArgumentOutOfRangeException(nameof(order), order, null)
        };

        var itemsInPage = sortedItems
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToList();

        return new Page<T>(
            itemsInPage,
            pageNumber,
            pageSize,
            itemsList.Count,
            (int)Math.Ceiling((decimal)itemsList.Count / pageSize));
    }
}