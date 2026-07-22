namespace LateApexEarlySpeed.Json.Schema.Keywords;

internal class AbsoluteUriFormatValidator : FormatValidator
{
    public const string FormatName = "uri";

    public override bool Validate(string content)
    {
        try
        {
            var _ = new Uri(content, UriKind.Absolute);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}