using Flvt.Infrastructure.Processors.AI.GPT.Utils;

namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Batches;

internal sealed record BatchMessageLine(
    string CustomId,
    object Body,
    string Method = "POST",
    string Url = GPTPaths.CreateCompletion);