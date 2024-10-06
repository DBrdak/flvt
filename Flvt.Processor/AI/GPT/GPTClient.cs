using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Flvt.Application.Abstractions;
using Flvt.Domain.Primitives;
using System.Text;
using Flvt.Processor.AI.GPT.Models;
using Flvt.Processor.AI.GPT.Models.Response;

namespace Flvt.Processor.AI.GPT;

internal class GPTClient
{
    private readonly HttpClient _httpClient;
    private readonly ILoggingService _loggingService;
    private readonly JsonSerializerSettings _serializerSettings = new()
    {
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        }
    };

    public GPTClient(HttpClient httpClient, ILoggingService loggingService)
    {
        _httpClient = httpClient;
        _loggingService = loggingService;
    }

    public async Task<Result<string>> GenerateContentAsync(string prompt, CancellationToken cancellationToken)
    {
        var requestBody = GPTRequestFactory.CreateRequest(prompt);

        var content = new StringContent(
            JsonConvert.SerializeObject(
                requestBody,
                Formatting.None,
                _serializerSettings),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync("", content, cancellationToken);

        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _loggingService.LogError(responseBody);

            return GPTErrors.RequestFailed;
        }

        var gptResponse = JsonConvert.DeserializeObject<ChatCompletionResponse>(responseBody);

        return gptResponse?.Response;
    }
}