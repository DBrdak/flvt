using System.Globalization;

namespace Flvt.Domain.Extensions;

public static class DateParser
{
    public static DateTime ParseDate(string date)
    {
        DateTime parsedDate = default;
            
        var parseResult = DateTime.TryParse(date, out parsedDate);

        if(!parseResult)
        {
            parseResult = DateTime.TryParseExact(
                date,
                "dd.MM.yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out parsedDate);
        }

        return parsedDate == default ?
            throw new ArgumentException($"Invalid date time format specified: {date}") :
            parsedDate;
    }
}