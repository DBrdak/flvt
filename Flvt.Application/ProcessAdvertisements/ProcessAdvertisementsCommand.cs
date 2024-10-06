using Flvt.Application.Messaging;

namespace Flvt.Application.ProcessAdvertisements;

public sealed record ProcessAdvertisementsCommand(
    string Location,
    int? MinPrice,
    int? MaxPrice,
    int? MinRooms,
    int? MaxRooms,
    int? MinArea,
    int? MaxArea) : ICommand;
