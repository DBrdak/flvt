namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Files;

internal sealed record FileModel(
    string Id,
    string Object,
    int Bytes,
    long CreatedAt,
    string Filename,
    string Purpose);