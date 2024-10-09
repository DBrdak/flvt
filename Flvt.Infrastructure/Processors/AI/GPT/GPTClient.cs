using System.Text;
using Flvt.Domain.Primitives;
using Flvt.Infrastructure.Monitoring;
using Flvt.Infrastructure.Processors.AI.GPT.Domain;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Assistants;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Assistants.Create.Response;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Assistants.List.Response;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Messages;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Runs;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Runs.CreateAndRunThread.Request;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Threads.ListMessages.Response;
using Flvt.Infrastructure.Processors.AI.GPT.Options;
using Flvt.Infrastructure.Processors.AI.GPT.Utils;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace Flvt.Infrastructure.Processors.AI.GPT;

internal class GPTClient
{
    private readonly GPTMonitor _gptMonitor;
    private readonly HttpClient _httpClient;
    private Assistant? _assistant;
    private readonly List<Run> _runs = [];
    private readonly List<Run> _completedRuns = [];
    private readonly List<MessageBody[]> _messages = [];
    private const int maxMessagesPerRequest = 32;

    public GPTClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _gptMonitor = new ();
    }

    public async Task<Result<IEnumerable<string>>> MessageAsync(
        AssistantVariant assistantVariant,
        IEnumerable<string> messages,
        CancellationToken cancellationToken = default)
    {
        var assistantResult = await EnsureAssistantCreatedAsync(assistantVariant);

        if (assistantResult.IsFailure)
        {
            return assistantResult.Error;
        }


        _messages.AddRange(messages.ToList()[..30].Select(GPTRequestFactory.CreateMessage).Chunk(maxMessagesPerRequest));
        // _messages.AddRange(messages.Select(GPTRequestFactory.CreateMessage).Chunk(maxMessagesPerRequest)); TODO TESTING

        var runCreateResult = await RunMessagesProcessingAsync();

        if (runCreateResult.IsFailure)
        {
            return runCreateResult.Error;
        }

        var runResult = await WaitForThreadsToCompleteAsync();

        if (runResult.IsFailure)
        {
            return runResult.Error;
        }

        return await ListRepliesAsync();
    }

    public async Task<Result<string>> EnsureAssistantCreatedAsync(AssistantVariant variant)
    {
        var listAssistantsResponse = await _httpClient.GetAsync(GPTPaths.Assistants);

        var responseBody = await listAssistantsResponse.Content.ReadAsStringAsync();

        if (!listAssistantsResponse.IsSuccessStatusCode)
        {
            Log.Logger.Error("Failed to list GPT assistants, response: {response}", responseBody);

            return GPTErrors.RequestFailed;
        }

        var listAssistantsBody = GPTJsonConvert.DeserializeObject<ListAssistantsResponse>(responseBody);

        if (listAssistantsBody is null)
        {
            Log.Logger.Error(
                "Failed to deserialize JSON from GPT Assistant List endpoint, response body: {body}",
                responseBody);

            return GPTErrors.RequestFailed;
        }

        var assistants = listAssistantsBody.Data;

        var assistant = assistants.FirstOrDefault(a => a.Name == variant.Name);

        if (assistant is null)
        {
            return await CreateAssistantAsync(variant);
        }

        _assistant = assistant.AsAssistant();
        return assistant.Id;
    }

    private async Task<Result<string>> CreateAssistantAsync(AssistantVariant variant)
    {

        var createAssistantResponse = await _httpClient.PostAsync(
            GPTPaths.Assistants,
            CreateRequestBody(AssistantFactory.CreateFromVariant(variant)));

        var responseBody = await createAssistantResponse.Content.ReadAsStringAsync();

        if (!createAssistantResponse.IsSuccessStatusCode)
        {
            Log.Logger.Error("Failed to create GPT {variant}, response: {response}", variant.Name, responseBody);

            return GPTErrors.RequestFailed;
        }

        var createAssistantBody = GPTJsonConvert.DeserializeObject<AssistantCreateResponse>(responseBody);

        if (createAssistantBody is null)
        {
            Log.Logger.Error(
                "Failed to deserialize JSON from GPT Assistant Create endpoint, response body: {body}",
                responseBody);

            return GPTErrors.RequestFailed;
        }

        _assistant = createAssistantBody.AsAssistant();

        return createAssistantBody.Id;
    }

    private async Task<Result> RunMessagesProcessingAsync()
    {
        var responses = await SendMessagesAsync();

        foreach (var response in responses)
        {
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Log.Logger.Error("Failed to create and run GPT Thread, response: {response}", responseBody);

                return GPTErrors.RequestFailed;
            }

            var run = GPTJsonConvert.DeserializeObject<Run>(responseBody);

            if (run is not null)
            {
                _runs.Add(run);
                continue;
            }

            Log.Logger.Error(
                "Failed to deserialize JSON from GPT Thread create and Run endpoint, response body: {body}",
                responseBody);

            return GPTErrors.RequestFailed;
        }

        return Result.Success();
    }

    private async Task<IEnumerable<HttpResponseMessage>> SendMessagesAsync()
    {
        var tasks = new List<Task<HttpResponseMessage>>();

        foreach (var messagesChunk in _messages)
        {
            var task = _httpClient.PostAsync(
                GPTPaths.RunThread, CreateRequestBody(new CreateAndRunThreadRequest(_assistant!.Id, new(messagesChunk))));

            tasks.Add(task);
        }

        return await Task.WhenAll(tasks);
    }

    private async Task<Result> WaitForThreadsToCompleteAsync()
    {
        var currentRunIndex = 0;
        var currentRun = _runs[currentRunIndex];

        do
        {
            var runRetrieveResponse = await _httpClient.GetAsync(GPTPaths.RetrieveRun(currentRun.ThreadId, currentRun.Id));

            var responseBody = await runRetrieveResponse.Content.ReadAsStringAsync();

            if (!runRetrieveResponse.IsSuccessStatusCode)
            {
                Log.Logger.Error("Failed to retrieve GPT Run, response: {response}", responseBody);

                return GPTErrors.RequestFailed;
            }

            var run = GPTJsonConvert.DeserializeObject<Run>(responseBody);

            if (run is null)
            {
                Log.Logger.Error(
                    "Failed to deserialize JSON from GPT Run retireve endpoint, response body: {body}",
                    responseBody);

                return GPTErrors.RequestFailed;
            }

            if (run.IsFailed)
            {
                Log.Logger.Error(
                    "Failed to complete run {runId}, error: {error} / reason: {reason} / details: {details}",
                    run.Id,
                    run.LastError?.Message,
                    run.IncompleteDetails?.Reason,
                    run.IncompleteDetails?.Details);

                return GPTErrors.RequestFailed;
            }

            if (!run.IsCompleted)
            {
                await Task.Delay(500);

                continue;
            }

            _gptMonitor.AddRun(run);
            _completedRuns.Add(run);
            currentRunIndex++;
            currentRun = _runs.ElementAtOrDefault(currentRunIndex);

            if(currentRun is null)
            {
                return Result.Success();
            }
        }
        while (true);
    }

    private async Task<Result<IEnumerable<string>>> ListRepliesAsync()
    {
        HashSet<string> messages = [];
        var after = string.Empty;
        var maxLimit = 100;
        var run = _completedRuns.First();
        var runIndex = 0;

        do
        {
            var listMessagesResponse = await _httpClient.GetAsync(
                GPTPaths.ListThreadMessages(
                    run.ThreadId,
                    run.Id,
                    after,
                    maxLimit));

            var responseBody = await listMessagesResponse.Content.ReadAsStringAsync();

            if (!listMessagesResponse.IsSuccessStatusCode)
            {
                Log.Logger.Error("Failed to list GPT Messages, response: {response}", responseBody);

                return GPTErrors.RequestFailed;
            }

            var messagesResponse = GPTJsonConvert.DeserializeObject<ListMessagesResponse>(responseBody);

            if (messagesResponse is null)
            {
                Log.Logger.Error(
                    "Failed to deserialize JSON from GPT List Messages endpoint, response body: {body}",
                    responseBody);

                return GPTErrors.RequestFailed;
            }

            var data = messagesResponse.Data.ToList();

            if (!data?.Any() ?? false)
            {
                break;
            }

            data?.Where(m => m.Role.ToLower() != "user").ToList()
                .ForEach(m => messages.Add(m.Content.FirstOrDefault()?.Text?.Value));

            after = data?.LastOrDefault()?.Id ?? string.Empty;

            runIndex++;
            run = _completedRuns.ElementAtOrDefault(runIndex);

            if (run is null)
            {
                break;
            }
        }
        while (true);

        return messages;
    }

    private static StringContent CreateRequestBody(object body)
    {
        return new StringContent(
            GPTJsonConvert.Serialize(body),
            Encoding.UTF8,
            "application/json");
    }
}