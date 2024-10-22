using System.Text.RegularExpressions;
using Flvt.Domain.Filters.Erros;
using Flvt.Domain.Primitives.Responses;

namespace Flvt.Domain.Filters;

public sealed record FilterName
{
    public string Value { get; init; }
    private const string pattern = @"^[\p{L}\p{N},./;'[\]{}:""<>?~!@#$%^&*()`_+\-=\|]{1,60}$";

    private FilterName(string value) => Value = value;

    public static Result<FilterName> Create(string value)
    {
        if (!Regex.IsMatch(value, pattern))
        {
            return FilterErrors.InvalidName;
        }

        return new FilterName(value);
    }
    public override string ToString() => $"{Value}";
}