namespace Flvt.Application.Abstractions;

public interface ICustodiansOrchestrator
{
    Task<IEnumerable<string>> FindOutdatedAdvertisementsAsync(IEnumerable<string> advertisementsLinks);
    Task<IEnumerable<string>> FindDuplicatedAdvertisementsAsync(IEnumerable<string> advertisementsLinks);
}