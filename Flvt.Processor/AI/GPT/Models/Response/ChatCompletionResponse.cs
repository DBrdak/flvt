namespace Flvt.Processor.AI.GPT.Models.Response;

internal sealed record ChatCompletionResponse(
    string Id,
    string Object,
    long Created,
    string Model,
    string SystemFingerprint,
    List<Choice> Choices,
    Usage Usage
)
{
    public string Response => Choices.FirstOrDefault()?.Message.Content ?? string.Empty;
}