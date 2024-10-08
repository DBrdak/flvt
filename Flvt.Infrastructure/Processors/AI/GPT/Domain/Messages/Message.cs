namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Messages;

internal sealed record Message(
    string Id,
    string Object,
    long CreatedAt,
    string ThreadId,
    string Status,
    IncompleteDetails? IncompleteDetails,
    long? CompletedAt,
    long? IncompleteAt,
    string Role,
    IReadOnlyList<Content> Content,
    string? AssistantId,
    string? RunId,
    IReadOnlyList<Attachment>? Attachments,
    Metadata Metadata
);

internal sealed record IncompleteDetails(
    string Reason,
    string? AdditionalInfo
);

internal sealed record Content(
    string Type,
    TextContent? Text,
    ImageContent? Image
);

internal sealed record TextContent(
    string Value,
    IReadOnlyList<string> Annotations
);

internal sealed record ImageContent(
    string Url,
    string AltText
);

internal sealed record Attachment(
    string FileId,
    string ToolName
);

internal sealed record Metadata(
    IReadOnlyDictionary<string, string> Data
);