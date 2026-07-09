using System.Diagnostics;
using System.Text.Json;
using LateApexEarlySpeed.Json.Schema.Common;
using LateApexEarlySpeed.Json.Schema.Keywords;

namespace LateApexEarlySpeed.Json.Schema;

public class JsonValidatorOptions
{
    /// <summary>
    /// See doc of <see cref="KeywordRegistry"/> for details.
    /// </summary>
    internal ValidationKeywordRegistry? InternalKeywordRegistry { get; private set; }

    /// <summary>
    /// Its nullability is same as <see cref="InternalKeywordRegistry"/>. 
    /// Caches schema-deserialization <see cref="JsonSerializerOptions"/> instances per dialect for this options instance.
    /// </summary>
    internal JsonSerializerOptionsCache? JsonSerializerOptionsCache { get; private set; }

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
    /// A keyword implementation is resolved per keyword name and dialect. This per <see cref="JsonValidatorOptions"/> level <see cref="ValidationKeywordRegistry"/> takes higher precedence than <see cref="ValidationKeywordRegistry.Global"/>: the global registry is only consulted when this registry has no implementation registered for that specific keyword name and dialect (even if the same keyword name is registered here for other dialects).
    /// </remarks>
    public ValidationKeywordRegistry KeywordRegistry
    {
        get
        {
            if (InternalKeywordRegistry is null)
            {
                InternalKeywordRegistry = new ValidationKeywordRegistry(2, false);

                Debug.Assert(JsonSerializerOptionsCache is null);
                JsonSerializerOptionsCache = new JsonSerializerOptionsCache(this);
            }

            return InternalKeywordRegistry;
        }
    }

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

/// <summary>
/// Caches schema-deserialization <see cref="JsonSerializerOptions"/> instances per dialect
/// for a single <see cref="JsonValidatorOptions"/> instance.
/// </summary>
internal class JsonSerializerOptionsCache
{
    /// <summary>
    /// The validator options that owns this cache.
    /// </summary>
    private readonly JsonValidatorOptions _parent;

    private readonly JsonSerializerOptions?[] _jsonSerializerOptionsCache = new JsonSerializerOptions[ValidationKeywordRegistry.SupportedDialectsCount];

    public JsonSerializerOptionsCache(JsonValidatorOptions parent)
    {
        _parent = parent;
    }

    public JsonSerializerOptions GetJsonSerializerOptions(DialectKind dialect)
    {
        return _jsonSerializerOptionsCache[(int)dialect] ??= new JsonSerializerOptions
        {
            Converters = { new JsonSchemaDeserializerContextMarkerConverter(_parent, dialect) }
        };
    }
}
