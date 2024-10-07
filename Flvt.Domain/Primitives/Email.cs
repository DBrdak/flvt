﻿using System.ComponentModel.DataAnnotations;

namespace Flvt.Domain.Primitives;

public sealed record Email
{
    public string Value { get; init; }

    private Email(string value)
    {
        Value = value;
    }

    public static Result<Email> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new Error("Email cannot be empty");
        }

        if (!new EmailAddressAttribute().IsValid(value))
        {
            return new Error("Email is not valid");
        }

        return new Email(value);
    }
}