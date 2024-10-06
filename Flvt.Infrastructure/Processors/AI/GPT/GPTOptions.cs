using Microsoft.Extensions.Options;

namespace Flvt.Infrastructure.Processors.AI.GPT;

public class GPTOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}