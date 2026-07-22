using LateApexEarlySpeed.Json.Schema.Common;

namespace LateApexEarlySpeed.Json.Schema.Keywords;

internal class JsonPointerFormatValidator : FormatValidator
{
    public const string FormatName = "json-pointer";

    public override bool Validate(string content)
    {
        return LinkedListBasedImmutableJsonPointer.Create(content) is not null;
    }
}