namespace Flvt.Domain.Advertisements;

public sealed record Filter
{
    private Filter _tempFilter;
    public string Location { get; private set; } = string.Empty;
    public int? MaxPrice { get; private set; }
    public int? MinRooms { get; private set; }
    public int? MaxRooms { get; private set; }
    public int? MinArea { get; private set; }
    public int? MaxArea { get; private set; }

    public Filter() => _tempFilter = this;

    public Filter InLocation(string location) => _tempFilter with { Location = location };
    public Filter ToPrice(int maxPrice) => _tempFilter with { MaxPrice = maxPrice };
    public Filter FromRooms(int minRooms) => _tempFilter with { MinRooms = minRooms };
    public Filter ToRooms(int maxRooms) => _tempFilter with { MaxRooms = maxRooms };
    public Filter FromArea(int minArea) => _tempFilter with { MinArea = minArea };
    public Filter ToArea(int maxArea) => _tempFilter with { MaxArea = maxArea };

    public Filter Build()
    {
        if (string.IsNullOrWhiteSpace(_tempFilter.Location))
        {
            throw new ArgumentException("Location is required");
        }

        if (_tempFilter.MaxArea is not null &&
            _tempFilter.MinArea is not null &&
            _tempFilter.MaxArea < _tempFilter.MinArea)
        {
            throw new ArgumentException("Max area must be greater than or equal to min area");
        }

        if (_tempFilter.MaxRooms is not null &&
            _tempFilter.MinRooms is not null &&
            _tempFilter.MaxRooms < _tempFilter.MinRooms)
        {
            throw new ArgumentException("Max rooms number must be greater than or equal to min rooms number");
        }

        var filter = _tempFilter;
        _tempFilter = null;
        return filter;
    }
}