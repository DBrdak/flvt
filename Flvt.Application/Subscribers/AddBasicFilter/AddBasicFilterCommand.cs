using Flvt.Application.Messaging;
using Flvt.Application.Subscribers.Models;

namespace Flvt.Application.Subscribers.AddBasicFilter;

public sealed record AddBasicFilterCommand(
    string Name,
    string City,
    decimal MinPrice,
    decimal MaxPrice,
    int MinRooms,
    int MaxRooms,
    decimal MinArea,
    decimal MaxArea) : ICommand<FilterModel>;
