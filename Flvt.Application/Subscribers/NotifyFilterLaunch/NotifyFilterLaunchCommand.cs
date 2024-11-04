using Flvt.Application.Messaging;

namespace Flvt.Application.Subscribers.NotifyFilterLaunch;

public sealed record NotifyFilterLaunchCommand(IEnumerable<string> FiltersIds) : ICommand;
