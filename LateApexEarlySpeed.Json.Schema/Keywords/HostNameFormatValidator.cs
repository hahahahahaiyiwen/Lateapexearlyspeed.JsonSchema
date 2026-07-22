namespace LateApexEarlySpeed.Json.Schema.Keywords;

internal class HostNameFormatValidator : FormatValidator
{
    public const string FormatName = "hostname";

    public override bool Validate(string content)
    {
        return Uri.CheckHostName(content) == UriHostNameType.Dns;
    }
}