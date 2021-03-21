using System;

namespace Xamasoft.JsonClassGenerator
{
    public interface IJsonClassGeneratorConfig
    {
        string       Namespace                  { get; set; }
        string       SecondaryNamespace         { get; set; }
        bool         UseFields                  { get; set; }
        bool         InternalVisibility         { get; set; }
        bool         ExplicitDeserialization    { get; set; }
        bool         NoHelperClass              { get; set; }
        string       MainClass                  { get; set; }
        bool         UseProperties              { get; set; }
        bool         UsePascalCase              { get; set; }
        
        /// <summary>Use the <see cref="Newtonsoft.Json.JsonPropertyAttribute"/> on generated C# class properties (as opposed to not rendering any attributes, or using <see cref="UseJsonPropertyName"/>).</summary>
        bool         UseJsonAttributes          { get; set; }

        /// <summary>Use the <c>[JsonPropertyName]</c> attribute on generated C# class properties (as opposed to not rendering any attributes, or using <see cref="UseJsonAttributes"/>).</summary>
        bool         UseJsonPropertyName        { get; set; }

        bool         UseNestedClasses           { get; set; }
        bool         ApplyObfuscationAttributes { get; set; }
        bool         SingleFile                 { get; set; }
        ICodeBuilder CodeWriter                 { get; set; }
        bool         HasSecondaryClasses        { get; }
        bool         AlwaysUseNullableValues    { get; set; }
        bool         UseNamespaces              { get; }
        bool         ExamplesInDocumentation    { get; set; }
        bool         ImmutableClasses           { get; set; }
        bool         NoSettersForCollections    { get; set; }

        bool ArrayAsList();
    }

    public static class JsonClassGeneratorConfigExtensions
    {
        /// <summary>Never returns <see langword="null"/>. Returns either &quot;<c>[JsonPropertyName(&quot;<paramref name="field"/>.<see cref="FieldInfo.JsonMemberName"/>&quot;)]</c>&quot; or &quot;<c>[JsonPropertyName(&quot;<paramref name="field"/>.<see cref="FieldInfo.JsonMemberName"/>&quot;)]</c>&quot; - or an empty string depending on <paramref name="config"/> and <see cref="FieldInfo.ContainsSpecialChars"/>.</summary>
        /// <param name="config">Required. Cannot be <see langword="null"/>.</param>
        /// <param name="field">Required. Cannot be <see langword="null"/>.</param>
        /// <returns></returns>
        public static string GetCSharpJsonAttributeCode(this IJsonClassGeneratorConfig config, FieldInfo field)
        {
            if (config is null) throw new ArgumentNullException(nameof(config));
            if (field is null) throw new ArgumentNullException(nameof(field));

           // if (config.UseJsonAttributes && config.UseJsonPropertyName) throw new ArgumentException(message: "Cannot use both " + nameof(config.UseJsonPropertyName) + " and " + nameof(config.UseJsonAttributes) + ".", paramName: nameof(config));

            

            string name = field.JsonMemberName;

            if (config.UseJsonPropertyName)
            {
                return "[JsonPropertyName(\"" + name + "\")]";
            }
            else if (config.UseJsonAttributes || field.ContainsSpecialChars)
            {
                return "[JsonProperty(\"" + name + "\")]";
            }
            else
            {
                return String.Empty;
            }
        }
    }
}
