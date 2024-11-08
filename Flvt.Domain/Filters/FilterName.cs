using System.Text.RegularExpressions;
using Flvt.Domain.Filters.Erros;
using Flvt.Domain.Primitives.Responses;

namespace Flvt.Domain.Filters;

public sealed record FilterName
{
    public string Value { get; init; }

    private FilterName(string value) => Value = value;

    public static Result<FilterName> Create(string value)
    {
        if (value.Length is < 31 and > 0 &&
            string.IsNullOrWhiteSpace(value) &&
            value.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || char.IsSeparator(c) || char.IsPunctuation(c)))
        {
            return FilterErrors.InvalidName;
        }

        return new FilterName(value.Trim());
    }
    public override string ToString() => $"{Value}";
}