using System.Text;
using Flvt.Domain.Primitives;
using Flvt.Infrastructure.Monitoring;
using Flvt.Infrastructure.Processors.AI.GPT.Domain;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Assistants;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Assistants.List.Response;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Messages;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Runs;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Runs.CreateAndRunThread.Request;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Threads.ListMessages.Response;
using Flvt.Infrastructure.Processors.AI.GPT.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace Flvt.Infrastructure.Processors.AI.GPT;

internal class GPTClient
{
    private readonly GPTMonitor _gptMonitor;
    private readonly HttpClient _httpClient;
    private Assistant? _assistant;
    private Run? _run;

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

        var runResult = await RunMessagesProcessingAsync(messages);

        if (runResult.IsFailure)
        {
            return runResult.Error;
        }

        do
        {
            var isRunCompletedResult = await WaitForThreadToCompleteAsync();

            if (isRunCompletedResult.IsFailure)
            {
                return isRunCompletedResult.Error;
            }

            var isRunCompleted = isRunCompletedResult.Value;

            if (isRunCompleted)
            {
                break;
            }
        }
        while (true);

        //TODO get messages
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

        var listAssistantsBody = JsonConvert.DeserializeObject<ListAssistantsResponse>(responseBody);

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

        _assistant = assistant;
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

        var createAssistantBody = JsonConvert.DeserializeObject<Assistant>(responseBody);

        if (createAssistantBody is null)
        {
            Log.Logger.Error(
                "Failed to deserialize JSON from GPT Assistant Create endpoint, response body: {body}",
                responseBody);

            return GPTErrors.RequestFailed;
        }

        _assistant = createAssistantBody;

        return createAssistantBody.Id;
    }

    private async Task<Result<string>> RunMessagesProcessingAsync(IEnumerable<string> messages)
    {
        var createAndRunThreadResponse = await _httpClient.PostAsync(
            GPTPaths.RunThread,
            CreateRequestBody(
                new CreateAndRunThreadRequest(
                    _assistant!.Id,
                    new(messages.Select(GPTRequestFactory.CreateMessage)))));

        var responseBody = await createAndRunThreadResponse.Content.ReadAsStringAsync();

        if (!createAndRunThreadResponse.IsSuccessStatusCode)
        {
            Log.Logger.Error("Failed to create and run GPT Thread, response: {response}", responseBody);

            return GPTErrors.RequestFailed;
        }

        _run = JsonConvert.DeserializeObject<Run>(responseBody);

        if (_run is not null)
        {
            return _run.Id;
        }
        
        Log.Logger.Error(
            "Failed to deserialize JSON from GPT Thread create and Run endpoint, response body: {body}",
            responseBody);

        return GPTErrors.RequestFailed;
    }

    private async Task<Result<bool>> WaitForThreadToCompleteAsync()
    {
        var runRetrieveResponse = await _httpClient.GetAsync(GPTPaths.RetrieveRun(_run.ThreadId, _run.Id));

        var responseBody = await runRetrieveResponse.Content.ReadAsStringAsync();

        if (!runRetrieveResponse.IsSuccessStatusCode)
        {
            Log.Logger.Error("Failed to retrieve GPT Run, response: {response}", responseBody);

            return GPTErrors.RequestFailed;
        }

        var run = JsonConvert.DeserializeObject<Run>(responseBody);

        if (run is null)
        {
            Log.Logger.Error(
                "Failed to deserialize JSON from GPT Run retireve endpoint, response body: {body}",
                responseBody);

            return GPTErrors.RequestFailed;
        }

        if (run.IsCompleted)
        {
            return true;
        }

        if (!run.IsFailed)
        {
            return false;
        }

        Log.Logger.Error(
            "Failed to complete run {runId}, error: {error} / reason: {reason} / details: {details}",
            run.Id,
            run.LastError?.Message,
            run.IncompleteDetails?.Reason,
            run.IncompleteDetails?.Details);

        return GPTErrors.RequestFailed;

    }

    private async Task<Result<IEnumerable<string>>> ListRepliesAsync()
    {
        HashSet<string> messages = [];
        var after = string.Empty;
        var maxLimit = 100;

        do
        {
            var listMessagesResponse = await _httpClient.GetAsync(
                GPTPaths.ListThreadMessages(
                    _run!.ThreadId,
                    _run.Id,
                    after,
                    maxLimit));

            var responseBody = await listMessagesResponse.Content.ReadAsStringAsync();

            if (!listMessagesResponse.IsSuccessStatusCode)
            {
                Log.Logger.Error("Failed to list GPT Messages, response: {response}", responseBody);

                return GPTErrors.RequestFailed;
            }

            var messagesResponse = JsonConvert.DeserializeObject<ListMessagesResponse>(responseBody);

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

            data?.Where(m => m.Role.ToLower() == "user").ToList()
                .ForEach(m => messages.Add(m.Content.FirstOrDefault()?.Text?.Value));

            after = data?.LastOrDefault()?.Id ?? string.Empty;
        }
        while (true);

        return messages;
    }

    private static StringContent CreateRequestBody(object body)
    {
        JsonSerializerSettings serializerSettings = new()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

        return new StringContent(
            JsonConvert.SerializeObject(
                body,
                Formatting.None,
                serializerSettings),
            Encoding.UTF8,
            "application/json");
    }
}