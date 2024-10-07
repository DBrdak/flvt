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

    public static string ToStandarizedString(this int num) => num switch
    {
        0 => "ZERO",
        1 => "ONE",
        2 => "TWO",
        3 => "THREE",
        4 => "FOUR",
        5 => "FIVE",
        6 => "SIX",
        7 => "SEVEN",
        8 => "EIGHT",
        9 => "NINE",
        _ => throw new ArgumentOutOfRangeException(
            nameof(num),
            $"Number {num} is to large to convert it to normalized string")
    };
}