using System.Text;
using Flvt.Domain.Primitives;
using Flvt.Infrastructure.Processors.AI.GPT.Models;
using Flvt.Infrastructure.Processors.AI.GPT.Models.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace Flvt.Infrastructure.Processors.AI.GPT;

internal class GPTClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerSettings _serializerSettings = new()
    {
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        }
    };

    public GPTClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
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
            Log.Logger.Error(responseBody);

            return GPTErrors.RequestFailed;
        }

        var gptResponse = JsonConvert.DeserializeObject<ChatCompletionResponse>(responseBody);

        return gptResponse?.Response;
    }
}