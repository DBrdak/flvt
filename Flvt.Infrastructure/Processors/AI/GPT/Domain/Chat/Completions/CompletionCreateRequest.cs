﻿namespace Flvt.Infrastructure.Processors.AI.GPT.Domain.Chat.Completions;

internal sealed record CompletionCreateRequest(
    IReadOnlyCollection<Message> Messages,
    string Model,
    bool? Store = null,
    IReadOnlyDictionary<string, string>? Metadata = null,
    double? FrequencyPenalty = null,
    IReadOnlyDictionary<int, int>? LogitBias = null,
    bool? Logprobs = null,
    int? TopLogprobs = null,
    int? MaxTokens = null,
    int? MaxCompletionTokens = null,
    int? N = null,
    double? PresencePenalty = null,
    object? ResponseFormat = null,
    int? Seed = null,
    string? ServiceTier = null,
    string[]? Stop = null,
    bool? Stream = null,
    object? StreamOptions = null,
    double? Temperature = null,
    double? TopP = null,
    IReadOnlyCollection<object>? Tools = null,
    object? ToolChoice = null,
    string? User = null
);