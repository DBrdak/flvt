using Flvt.Application.Messaging;

namespace Flvt.Application.Subscribers.AddBasicFilter;

//TODO Consider authorization
public sealed record AddBasicFilterCommand(
    string Email,
    string Location,
    decimal MinPrice,
    decimal MaxPrice,
    int MinRooms,
    int MaxRooms,
    decimal MinArea,
    decimal MaxArea) : ICommand;
