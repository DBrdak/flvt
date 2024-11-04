namespace Flvt.API.Functions.API.Subscribers.Requests;

public sealed record AddBasicFilterRequest(
    string Name,
    string City,
    decimal? MinPrice,
    decimal? MaxPrice,
    int? MinRooms,
    int? MaxRooms,
    decimal? MinArea,
    decimal? MaxArea);