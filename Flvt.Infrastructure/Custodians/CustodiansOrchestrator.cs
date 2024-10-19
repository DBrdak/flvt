using Flvt.Application.Abstractions;

namespace Flvt.Infrastructure.Custodians;

internal class CustodiansOrchestrator : ICustodiansOrchestrator
{
    public async Task<IEnumerable<string>> FindOutdatedAdvertisementsAsync(IEnumerable<string> advertisementsLinks)
    {
        return null;
    }

    public async Task<IEnumerable<string>> FindDuplicatedAdvertisementsAsync(IEnumerable<string> advertisementsLinks)
    {
        return null;
    }
}