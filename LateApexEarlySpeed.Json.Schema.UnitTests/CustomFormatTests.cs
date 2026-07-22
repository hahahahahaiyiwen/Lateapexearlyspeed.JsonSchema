using LateApexEarlySpeed.Json.Schema.Keywords;
using Xunit;

namespace LateApexEarlySpeed.Json.Schema.UnitTests;

public class CustomFormatTests
{
    [Fact]
    public void RegisterCustomFormat()
    {
        FormatRegistry formatRegistry = FormatRegistry.CreateDefaultRegistry();

        formatRegistry.AddFormat("custom_format", () => new TrueToFalseFormatValidator());
        
        // Test to add duplicated format names
        formatRegistry.SetFormat("custom_format", () => new TrueToTrueFormatValidator());
        Assert.Throws<ArgumentException>(() => formatRegistry.AddFormat("custom_format", () => new TrueToFalseFormatValidator()));
        Assert.Throws<ArgumentException>(() => formatRegistry.AddFormat(DateTimeFormatValidator.FormatName, () => new DateTimeFormatValidator()));
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    public void Validate_WithGlobalCustomFormat(string value, bool expectedResult)
    {
        string schema = """{"format": "custom_format"}""";
        string instance = '\"' + value + '\"';

        FormatRegistry globalRegistry = FormatRegistry.CreateDefaultRegistry();
        globalRegistry.AddFormat("custom_format", () => new TrueToTrueFormatValidator());

        var options = new JsonValidatorOptions { GlobalFormatRegistry = globalRegistry };
        Assert.Equal(expectedResult, new JsonValidator(schema, options).Validate(instance).IsValid);
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    public void Validate_WithOptionLevelCustomFormat(string value, bool expectedResult)
    {
        string schema = """{"format": "custom_format"}""";
        string instance = '\"' + value + '\"';

        var options = new JsonValidatorOptions();
        options.FormatRegistry.AddFormat("custom_format", () => new TrueToTrueFormatValidator());
        Assert.Equal(expectedResult, new JsonValidator(schema, options).Validate(instance).IsValid);
    }

    [Theory]
    [InlineData("true", "false", true)]
    [InlineData("true", "true", false)]
    [InlineData("false", "false", false)]
    [InlineData("false", "true", false)]
    public void Validate_DifferentFormatName_ForGlobalAndOptionLevelRegistry(string propAValue, string propBValue, bool expectedResult)
    {
        string schema = """
                        {
                          "properties": {
                            "a": { "format": "true_to_true" },
                            "b": { "format": "true_to_false" }
                          }
                        }
                        """;
        string instance = $$"""
                            { 
                              "a": "{{propAValue}}",
                              "b": "{{propBValue}}"
                            }
                            """;

        FormatRegistry globalRegistry = FormatRegistry.CreateDefaultRegistry();
        globalRegistry.AddFormat("true_to_true", () => new TrueToTrueFormatValidator());

        var options = new JsonValidatorOptions { GlobalFormatRegistry = globalRegistry };
        options.FormatRegistry.AddFormat("true_to_false", () => new TrueToFalseFormatValidator());
        Assert.Equal(expectedResult, new JsonValidator(schema, options).Validate(instance).IsValid);
    }

    [Theory]
    [InlineData("false", "false", true)]
    [InlineData("true", "true", false)]
    [InlineData("true", "false", false)]
    [InlineData("false", "true", false)]
    public void Validate_SameFormatName_OptionLevelShouldOverrideGlobalLevel(string propAValue, string propBValue, bool expectedResult)
    {
        string schema = """
                        {
                          "properties": {
                            "a": { 
                              "$id": "https://example.com/schema-a",
                              "$schema": "https://json-schema.org/draft/2019-09/schema",
                              "format": "custom_format" 
                            },
                            "b": {
                              "$id": "https://example.com/schema-b",
                              "$schema": "http://json-schema.org/draft-07/schema#",
                              "format": "custom_format"
                            }
                          }
                        }
                        """;
        string instance = $$"""
                            { 
                              "a": "{{propAValue}}",
                              "b": "{{propBValue}}"
                            }
                            """;

        FormatRegistry globalRegistry = FormatRegistry.CreateDefaultRegistry();
        globalRegistry.AddFormat("custom_format", () => new TrueToTrueFormatValidator());

        var options = new JsonValidatorOptions { GlobalFormatRegistry = globalRegistry };
        options.FormatRegistry.AddFormat("custom_format", () => new TrueToFalseFormatValidator());
        Assert.Equal(expectedResult, new JsonValidator(schema, options).Validate(instance).IsValid);
    }

    [Theory]
    [InlineData("true", true, false)]
    [InlineData("false", false, true)]
    public void Validate_WithGlobalCustomFormat_RegistriesAreIsolated(string value, bool expectedResultForTrueToTrueRegistry, bool expectedResultForTrueToFalseRegistry)
    {
        string schema = """{"format": "custom_format"}""";
        string instance = "\"" + value + "\"";

        FormatRegistry trueToTrueRegistry = FormatRegistry.CreateDefaultRegistry();
        trueToTrueRegistry.AddFormat("custom_format", () => new TrueToTrueFormatValidator());

        FormatRegistry trueToFalseRegistry = FormatRegistry.CreateDefaultRegistry();
        trueToFalseRegistry.AddFormat("custom_format", () => new TrueToFalseFormatValidator());

        var optionsWithTrueToTrueRegistry = new JsonValidatorOptions
        {
            GlobalFormatRegistry = trueToTrueRegistry
        };

        var optionsWithTrueToFalseRegistry = new JsonValidatorOptions
        {
            GlobalFormatRegistry = trueToFalseRegistry
        };

        Assert.Equal(expectedResultForTrueToTrueRegistry, new JsonValidator(schema, optionsWithTrueToTrueRegistry).Validate(instance).IsValid);
        Assert.Equal(expectedResultForTrueToFalseRegistry, new JsonValidator(schema, optionsWithTrueToFalseRegistry).Validate(instance).IsValid);
    }
}