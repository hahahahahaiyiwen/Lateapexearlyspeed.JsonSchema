using System.Globalization;

namespace LateApexEarlySpeed.Json.Schema.Keywords;

internal class DateFormatValidator : FormatValidator
{
    public const string FormatName = "date";

    public override bool Validate(string content)
    {
        return DateTimeOffset.TryParseExact(content, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
    }
}