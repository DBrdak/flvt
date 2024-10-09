﻿using Flvt.Domain.Primitives;

namespace Flvt.Infrastructure.Processors.AI;

internal static class AIProcessorErrors
{
    public static Error DeserializationError => new ("AI processor failed to convert prompt to processed advertisement");
}