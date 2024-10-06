using Flvt.Application.Abstractions;
using Flvt.Domain.Advertisements;
using Flvt.Domain.Advertisements.Errors;
using Flvt.Domain.Primitives;
using Flvt.Infrastructure.Processors.AI;
using Flvt.Infrastructure.Processors.AI.Models;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Serilog;

namespace Flvt.Infrastructure.Processors;

internal sealed class ProcessingOrchestrator : IProcessingOrchestrator
{
    private readonly IProcessedAdvertisementRepository _processedAdvertisementRepository;
    private readonly AIProcessor _aiProcessor;
    private readonly List<ProcessedAdvertisement> _processedAdvertisements = [];
    private readonly List<ScrapedAdvertisement> _advertisementsToProcess = [];

    public ProcessingOrchestrator(IProcessedAdvertisementRepository processedAdvertisementRepository, AIProcessor aiProcessor)
    {
        _processedAdvertisementRepository = processedAdvertisementRepository;
        _aiProcessor = aiProcessor;
    }

    public async Task ProcessAsync(IEnumerable<ScrapedAdvertisement> scrapedAdvertisements)
    {
        var existingAdvertisementsGetResult = await _processedAdvertisementRepository.GetAllAsync(CancellationToken.None);
        List<ProcessedAdvertisement> processedAdvertisements = [];

        if (existingAdvertisementsGetResult.IsSuccess)
        {
            processedAdvertisements = existingAdvertisementsGetResult.Value.ToList();
        }

        FindNotProcessedAdvertisements(scrapedAdvertisements, processedAdvertisements);

        var prompts = await SendPromptsAsync();

        prompts.ToList().ForEach(ProcessPromptAsync);

        var addResult = await _processedAdvertisementRepository.AddRangeAsync(_processedAdvertisements, CancellationToken.None);

        if (addResult.IsFailure)
        {
            Log.Logger.Error("Failed to add processed advertisements to the database: {error}", addResult.Error);
        }
    }

    private void ProcessPromptAsync(Result<string> prompt)
    {
        if (prompt.IsFailure)
        {
            Log.Logger.Error("AI processor failed to process advertisement: {error}", prompt.Error);
            return;
        }

        var semiProcessedAdvertisement = JsonConvert.DeserializeObject<SemiProcessedAdvertisement>(ResponseAsJson(prompt.Value));

        var advertisement = _advertisementsToProcess.First(ad => ad.Link == prompt.Value);

        if (semiProcessedAdvertisement is null)
        {
            Log.Logger.Error("AI processor failed to convert processed advertisement ({link})", advertisement.Link);
            return;
        }

        var processedAdvertisement = new ProcessedAdvertisement(
            advertisement,
            semiProcessedAdvertisement.ShortDescription,
            semiProcessedAdvertisement.Price,
            semiProcessedAdvertisement.NotesForPrice,
            semiProcessedAdvertisement.Deposit,
            semiProcessedAdvertisement.AvailableFrom,
            semiProcessedAdvertisement.Facilities,
            null);

        _processedAdvertisements.Add(processedAdvertisement);
    }

    private async Task<IEnumerable<Result<string>>> SendPromptsAsync()
    {
        var promptTasks = new List<Task<Result<string>>>();

        foreach (var advertisement in _advertisementsToProcess)
        {
            var prompt = PromptFactory.CreateDefaultAdvertisementProcessingPrompt(advertisement);

            promptTasks.Add(_aiProcessor.SendPromptAsync(prompt, CancellationToken.None));
        }

        await Task.WhenAll(promptTasks);

        return promptTasks.Select(p => p.Result);
    }

    private string ResponseAsJson(string response) =>
        response.Substring(response.IndexOf('{'), response.IndexOf('{') - response.LastIndexOf('}') + 1);

    private void FindNotProcessedAdvertisements(
        IEnumerable<ScrapedAdvertisement> scrapedAdvertisements,
        IEnumerable<ProcessedAdvertisement> processedAdvertisements)
    {
        foreach (var scrapedAdvertisement in scrapedAdvertisements)
        {
            var processedAdvertisement = processedAdvertisements.ToList().FirstOrDefault(
                ad => ad.Link == scrapedAdvertisement.Link && scrapedAdvertisement.UpdatedAt == ad.UpdatedAt);

            switch (processedAdvertisement)
            {
                case null:
                    _advertisementsToProcess.Add(scrapedAdvertisement);
                    break;
                default:
                    _processedAdvertisements.Add(processedAdvertisement);
                    break;
            }
        }
    }
}