using System.Reflection;

namespace LateApexEarlySpeed.Json.Schema.Keywords;

public class FormatRegistry
{
    private readonly Dictionary<string, Type> _formatValidatorTypes = new();

    internal FormatRegistry(bool withBuiltInFormats)
    {
        if (!withBuiltInFormats)
        {
            return;
        }

        Type[] builtInFormatTypes = new[]
        {
            typeof(DateTimeFormatValidator),
            typeof(TimeFormatValidator),
            typeof(DateFormatValidator),
            typeof(EmailFormatValidator),
            typeof(HostNameFormatValidator),
            typeof(IPv4FormatValidator),
            typeof(IPv6FormatValidator),
            typeof(GuidFormatValidator),
            typeof(AbsoluteUriFormatValidator),
            typeof(UriReferenceFormatValidator),
            typeof(JsonPointerFormatValidator),
            typeof(RegexFormatValidator)
        };

        foreach (Type formatType in builtInFormatTypes)
        {
            AddFormatType(formatType);
        }
    }

    public static FormatRegistry Global { get; } = new(true);

    /// <summary>
    /// Add new format type <typeparamref name="TFormatValidator"/> to <see cref="FormatRegistry"/>
    /// </summary>
    /// <typeparam name="TFormatValidator">New format type to be added</typeparam>
    /// <exception cref="ArgumentException">A format type with the same name already exists in the <see cref="FormatRegistry"/></exception>
    public void AddFormatType<TFormatValidator>() where TFormatValidator : FormatValidator
    {
        AddFormatType(typeof(TFormatValidator));
    }

    private void AddFormatType(Type formatValidatorType)
    {
        _formatValidatorTypes.Add(GetFormatName(formatValidatorType), formatValidatorType);
    }

    /// <summary>
    /// Set new format type <typeparamref name="TFormatValidator"/> to <see cref="FormatRegistry"/>.
    /// If specified format name does not exist, it is added; otherwise it is updated with new format type.
    /// </summary>
    /// <typeparam name="TFormatValidator">New format type to be set</typeparam>
    public void SetFormatType<TFormatValidator>() where TFormatValidator : FormatValidator
    {
        _formatValidatorTypes[GetFormatName(typeof(TFormatValidator))] = typeof(TFormatValidator);
    }

    private static string GetFormatName(Type formatValidatorType)
    {
        FormatAttribute? formatAttribute = formatValidatorType.GetCustomAttribute<FormatAttribute>();
        if (formatAttribute is null)
        {
            throw new ArgumentException($"Argument: {formatValidatorType.FullName} should contain {nameof(FormatAttribute)}.", nameof(formatValidatorType));
        }

        return formatAttribute.Name;
    }

    /// <returns>Return <see cref="Type"/> for <paramref name="format"/> keyword if registered; otherwise return null</returns>
    public Type? GetFormatType(string format)
    {
        return _formatValidatorTypes.GetValueOrDefault(format);
    }
}