﻿using System.Globalization;

namespace Flvt.Domain.Extensions;

public static class DateParser
{
    public static DateTime ParseDate(string date)
    {
        var parseResult = DateTime.TryParse(date, out var parsedDate);

        if(!parseResult)
        {
            parseResult = DateTime.TryParseExact(
                date,
                "d.MM.yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out parsedDate);
        }

        if (!parseResult)
        {
            var culture = new CultureInfo("pl-PL");
            var format = "dd MMMM yyyy";

            parseResult = DateTime.TryParseExact(
                date,
                format,
                culture,
                DateTimeStyles.None,
                out parsedDate);
        }

        return parsedDate == default ?
            throw new ArgumentException($"Invalid date time format specified: {date}") :
            parsedDate;
    }
}