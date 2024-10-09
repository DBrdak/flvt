using System.Text;
using Flvt.Infrastructure.Monitoring;
using Flvt.Infrastructure.Processors.AI.GPT.Domain.Chat.Completions;
using Flvt.Infrastructure.Processors.AI.GPT.Utils;
using Serilog;

namespace Flvt.Infrastructure.Processors.AI.GPT;

internal sealed class GPTClient
{
    private readonly HttpClient _httpClient;
    private readonly GPTMonitor _monitor;

    public GPTClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _monitor = new ();
    }

    public async Task<string?> CreateCompletionAsync(Message message, GPTModel model)
    {
        var completionRequest = new CompletionCreateRequest(
            Messages: [message],
            Model: model.Value,
            Store: true);

        var response = await _httpClient.PostAsync(GPTPaths.CreateCompletion, CreateHttpBody(completionRequest));
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode) //TODO check if the error is due to rate limit
        {
            Log.Logger.Error("Failed to create GPT Completion, response: {response}", responseContent);

            return null;
        }

        var completion = GPTJsonConvert.DeserializeObject<ChatCompletion>(responseContent);

        if (completion is null)
        {
            Log.Logger.Error("Failed to deserialize GPT Completion on response response: {response}", responseContent);

            return null;
        }

        _monitor.AddCompletion(completion);
        var chatResponse = completion.Choices.FirstOrDefault()?.Message.Content;

        if (chatResponse is null)
        {
            Log.Logger.Error("Failed to get chat response from GPT Completion, completion: {completion}", completion);

            return null;
        }


        return ToJson(chatResponse);
    }

    private static string ToJson(string chatResponse) =>
        chatResponse.Substring(
            chatResponse.IndexOf('{'),
            chatResponse.LastIndexOf('}') - chatResponse.IndexOf('{') + 1);

    private static StringContent CreateHttpBody<T>(T obj) => 
        new(GPTJsonConvert.Serialize(obj), Encoding.UTF8, "application/json");
}