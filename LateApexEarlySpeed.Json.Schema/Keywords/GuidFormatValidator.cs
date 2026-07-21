namespace LateApexEarlySpeed.Json.Schema.Keywords;

internal class GuidFormatValidator : FormatValidator
{
    public const string FormatName = "uuid";

    public override bool Validate(string content)
    {
        return Guid.TryParse(content, out _);
    }
}