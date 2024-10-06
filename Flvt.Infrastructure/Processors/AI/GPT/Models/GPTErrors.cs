﻿using Flvt.Domain.Primitives;

namespace Flvt.Infrastructure.Processors.AI.GPT.Models;

internal static class GPTErrors
{
    public static Error RequestFailed => new("Problem occurred while making the request to the GPT model");
}