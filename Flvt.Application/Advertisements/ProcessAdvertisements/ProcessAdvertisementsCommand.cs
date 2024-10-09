using Flvt.Application.Messaging;

namespace Flvt.Application.Advertisements.ProcessAdvertisements;

public sealed record ProcessAdvertisementsCommand(
    string FilterName,
    string Location,
    int? MinPrice,
    int? MaxPrice,
    int? MinRooms,
    int? MaxRooms,
    int? MinArea,
    int? MaxArea) : ICommand;
