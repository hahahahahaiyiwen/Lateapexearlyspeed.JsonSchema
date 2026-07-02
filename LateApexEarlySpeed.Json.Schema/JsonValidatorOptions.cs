using LateApexEarlySpeed.Json.Schema.Keywords;

namespace LateApexEarlySpeed.Json.Schema;

public class JsonValidatorOptions
{
    /// <summary>
    /// See doc of <see cref="KeywordRegistry"/> for details.
    /// </summary>
    internal ValidationKeywordRegistry? InternalKeywordRegistry;

    /// <summary>
    /// Gets or sets a value that determines whether a property's name uses a case-insensitive comparison during validation. The default value is false.
    /// </summary>
    /// <returns>
    /// true to compare property names using case-insensitive comparison; otherwise, false.
    /// </returns>
    public bool PropertyNameCaseInsensitive { get; set; }

    /// <summary>
    /// Gets or sets a value that determines whether removing json schema resource id from unknown keywords. The default value is false.
    /// </summary>
    public bool IgnoreResourceIdInUnknownKeyword { set; get; }

    /// <summary>
    /// Gets or sets a value that determines default dialect when there is no '$schema' identifier in Json schema. The default value is <see cref="DialectKind.Draft202012"/>.
    /// </summary>
    public DialectKind DefaultDialect { get; set; }

    /// <summary>
    /// Gets the <see cref="ValidationKeywordRegistry"/> instance that registers and retrieves validation keywords.
    /// </summary>
    /// <remarks>
    /// This per <see cref="JsonValidatorOptions"/> level <see cref="ValidationKeywordRegistry"/> takes higher precedence than <see cref="ValidationKeywordRegistry.Global"/> when resolving keyword implementation on same keyword name.
    /// </remarks>
    public ValidationKeywordRegistry KeywordRegistry => InternalKeywordRegistry ??= new ValidationKeywordRegistry(false);

    internal static JsonValidatorOptions Default { get; } = new();

    internal bool Equals(JsonValidatorOptions other)
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return PropertyNameCaseInsensitive == other.PropertyNameCaseInsensitive 
               && IgnoreResourceIdInUnknownKeyword == other.IgnoreResourceIdInUnknownKeyword
               && DefaultDialect == other.DefaultDialect
               && ReferenceEquals(InternalKeywordRegistry, other.InternalKeywordRegistry);
    }
}
