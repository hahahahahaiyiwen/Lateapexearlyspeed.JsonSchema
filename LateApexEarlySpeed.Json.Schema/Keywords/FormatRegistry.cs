namespace LateApexEarlySpeed.Json.Schema.Keywords;

public class FormatRegistry
{
    private readonly Dictionary<string, FastLazy> _formatValidators = new();

    internal FormatRegistry(bool withBuiltInFormats)
    {
        if (!withBuiltInFormats)
        {
            return;
        }

        var builtInFormats = new Dictionary<string, Func<FormatValidator>>
        {
            { DateTimeFormatValidator.FormatName, () => new DateTimeFormatValidator() },
            { TimeFormatValidator.FormatName, () => new TimeFormatValidator() },
            { DateFormatValidator.FormatName, () => new DateFormatValidator() },
            { EmailFormatValidator.FormatName, () => new EmailFormatValidator() },
            { HostNameFormatValidator.FormatName, () => new HostNameFormatValidator() },
            { IPv4FormatValidator.FormatName, () => new IPv4FormatValidator() },
            { IPv6FormatValidator.FormatName, () => new IPv6FormatValidator() },
            { GuidFormatValidator.FormatName, () => new GuidFormatValidator() },
            { AbsoluteUriFormatValidator.FormatName, () => new AbsoluteUriFormatValidator() },
            { UriReferenceFormatValidator.FormatName, () => new UriReferenceFormatValidator() },
            { JsonPointerFormatValidator.FormatName, () => new JsonPointerFormatValidator() },
            { RegexFormatValidator.FormatName, () => new RegexFormatValidator() }
        };

        foreach (KeyValuePair<string, Func<FormatValidator>> format in builtInFormats)
        {
            AddFormat(format.Key, format.Value);
        }
    }

    public static FormatRegistry Global { get; } = CreateDefaultRegistry();

    internal static FormatRegistry CreateDefaultRegistry() => new(true);

    /// <summary>
    /// Add a new format validator factory to <see cref="FormatRegistry"/>.
    /// </summary>
    /// <param name="formatName">The format name to register.</param>
    /// <param name="formatValidatorFactory">The factory used to create the format validator.</param>
    /// <exception cref="ArgumentException">A format validator with the same format name already exists in the <see cref="FormatRegistry"/></exception>
    /// <remarks>
    /// The factory is evaluated lazily. The created validator is cached and reused by this registry, so custom format validators should be stateless and thread-safe.
    /// </remarks>
    public void AddFormat(string formatName, Func<FormatValidator> formatValidatorFactory)
    {
        _formatValidators.Add(formatName, new FastLazy(formatValidatorFactory));
    }

    /// <summary>
    /// Set a format validator factory to <see cref="FormatRegistry"/>.
    /// If specified format name does not exist, it is added; otherwise it is updated with the new format validator factory.
    /// </summary>
    /// <param name="formatName">The format name to add or update.</param>
    /// <param name="formatValidatorFactory">The factory used to create the format validator.</param>
    /// <remarks>
    /// The factory is evaluated lazily. The created validator is cached and reused by this registry, so custom format validators should be stateless and thread-safe.
    /// </remarks>
    public void SetFormat(string formatName, Func<FormatValidator> formatValidatorFactory)
    {
        _formatValidators[formatName] = new FastLazy(formatValidatorFactory);
    }

    /// <summary>
    /// Gets the registered format validator for the specified format name.
    /// </summary>
    /// <param name="format">The format name to resolve.</param>
    /// <returns>Return <see cref="FormatValidator"/> for <paramref name="format"/> keyword if registered; otherwise return null</returns>
    public FormatValidator? GetFormatValidator(string format)
    {
        return _formatValidators.TryGetValue(format, out FastLazy? lazy) ? lazy.Value : null;
    }

    /// <remarks>
    /// This Lazy implementation is used to avoid the overhead of using Lazy which uses locks and is thread-safe to ensure only one instance is created.
    /// In this case, we don't need to ensure only one instance is created, so we can use a simpler implementation that doesn't use locks.
    /// </remarks>
    private class FastLazy
    {
        private volatile FormatValidator? _formatValidator;

        private readonly Func<FormatValidator> _formatValidatorFactory;

        public FastLazy(Func<FormatValidator> formatValidatorFactory)
        {
            _formatValidatorFactory = formatValidatorFactory;
        }

        public FormatValidator Value
        {
            get
            {
                if (_formatValidator is null)
                {
                    _formatValidator = _formatValidatorFactory();
                }

                return _formatValidator;
            }
        }
    }
}