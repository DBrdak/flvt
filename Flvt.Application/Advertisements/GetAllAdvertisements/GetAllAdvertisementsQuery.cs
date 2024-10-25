using Flvt.Application.Advertisements.Models;
using Flvt.Application.Messaging;

namespace Flvt.Application.Advertisements.GetAllAdvertisements;

public sealed record GetAllAdvertisementsQuery : IQuery<IEnumerable<ProcessedAdvertisementModel>>;