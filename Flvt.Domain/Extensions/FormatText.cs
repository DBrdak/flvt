namespace Flvt.Domain.Extensions;

public static class FormatText
{
    public static string ReplacePolishCharacters(this string text) => text
        .Replace("ą", "a")
        .Replace("ć", "c")
        .Replace("ę", "e")
        .Replace("ł", "l")
        .Replace("ń", "n")
        .Replace("ó", "o")
        .Replace("ś", "s")
        .Replace("ź", "z")
        .Replace("ż", "z")
        .Replace("Ą", "A")
        .Replace("Ć", "C")
        .Replace("Ę", "E")
        .Replace("Ł", "L")
        .Replace("Ń", "N")
        .Replace("Ó", "O")
        .Replace("Ś", "S")
        .Replace("Ź", "Z")
        .Replace("Ż", "Z");
}