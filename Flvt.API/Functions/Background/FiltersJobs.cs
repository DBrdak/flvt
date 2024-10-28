using Amazon.Lambda.Annotations;
using Flvt.Application.Subscribers.LaunchFilters;
using MediatR;

namespace Flvt.API.Functions.Background;

internal class FiltersJobs : BaseFunction
{
    public FiltersJobs(ISender sender) : base(sender)
    {
    }

    [LambdaFunction(ResourceName = nameof(LaunchFilters))]
    public async Task LaunchFilters()
    {
        var command = new LaunchFiltersCommand();

        _ = await Sender.Send(command);
    }
}