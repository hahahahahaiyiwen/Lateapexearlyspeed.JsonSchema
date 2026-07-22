using System.Text.RegularExpressions;
using LateApexEarlySpeed.Json.Schema.Common;

namespace LateApexEarlySpeed.Json.Schema.Keywords;

internal class RegexFormatValidator : FormatValidator
{
    public const string FormatName = "regex";

    public override bool Validate(string content)
    {
        try
        {
            var _ = RegexFactory.Create(content, RegexOptions.None);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}