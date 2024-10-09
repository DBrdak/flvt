using Flvt.Domain.Primitives.Responses;

namespace Flvt.Infrastructure.Processors.AI.GPT.Domain;

internal static class GPTErrors
{
    public static Error RequestFailed => new("Problem occurred while making the request to the GPT model");
}