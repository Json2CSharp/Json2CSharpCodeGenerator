using System;

namespace Xamasoft.JsonClassGenerator
{
    public interface IJsonClassGeneratorConfig
    {
        /// <summary>
        /// The C# <c>namespace</c> or Java <c>package</c> that the generated types will reside in.<br />
        /// <see langword="null"/> by default. If null/empty/whitespace then no enclosing namespace will be written in the output.
        /// </summary>
        string       Namespace                  { get; set; }

        /// <summary>
        /// The C# <c>namespace</c> or Java <c>package</c> that &quot;secondary&quot; generated types will reside in.<br />
        /// <see langword="null"/> by default.
        /// </summary>
        string       SecondaryNamespace         { get; set; }

        OutputTypes          OutputType         { get; set; }
        OutputCollectionType CollectionType     { get; set; }

        /// <summary>Options contained within are only respected when <see cref="IJsonClassGeneratorConfig.OutputType"/> == <see cref="OutputTypes.MutableClass"/>.</summary>
        MutableClassConfig   MutableClasses     { get; }

        JsonLibrary                AttributeLibrary { get; set; }
        JsonPropertyAttributeUsage AttributeUsage   { get; set; }

        bool         InternalVisibility         { get; set; }
        bool         NoHelperClass              { get; set; }

        /// <summary>
        /// When true, then the generated C# and VB.NET classes will have PascalCase property names, which means that <c>[JsonProperty]</c> or <c>[JsonPropertyName]</c> will be applied to all properties if the source JSON uses camelCase names, regardless of <see cref="AttributeUsage"/>.<br />
        /// When false, then the generated C# and VB.NET classes' property names will use the same names as the source JSON (except when a JSON property name cannot be represented by a C# identifier).
        /// </summary>
        bool         UsePascalCase              { get; set; }

        bool         UseNestedClasses           { get; set; }

        /// <summary>Name of the outer class. Only used when <c><see cref="UseNestedClasses"/> == <see langword="true"/></c>.</summary>
        string       MainClass                  { get; set; }

        /// <summary>When <see langword="true"/>, then <see cref="System.Reflection.ObfuscationAttribute"/> will be applied to generated types.</summary>
        bool         ApplyObfuscationAttributes { get; set; }

        bool         SingleFile                 { get; set; }
        ICodeBuilder CodeWriter                 { get; set; }
        bool         AlwaysUseNullableValues    { get; set; }
        bool         ExamplesInDocumentation    { get; set; }
    }

    public enum OutputTypes
    {
        /// <summary>
        /// C#: Mutable <c>class</c> types.<br />
        /// VB.NET: Mutable <c>Class</c> types.<br />
        /// Java: Mutable Bean <c>class</c>
        /// </summary>
        MutableClass,

        /// <summary>
        /// C#: Immutable <c>class</c> types. Using <c>[JsonConstructor]</c>.<br />
        /// VB.NET: Immutable <c>Class</c> types. Using <c>[JsonConstructor]</c>.<br />
        /// Java: Not yet implemented. TODO.
        /// </summary>
        ImmutableClass,

        /// <summary>
        /// C#: Immutable <c>record</c> types.<br />
        /// VB.NET: Not supported.<br />
        /// Java: Not supported.<br />
        /// </summary>
        ImmutableRecord
    }

    public enum OutputCollectionType
    {
        /// <summary>Expose collections as <c>T[]</c>.</summary>
        Array,

        /// <summary>
        /// C#/VB.NET: Expose collections as <c><see cref="System.Collections.Generic.List{T}"/></c>.<br />
        /// Java: Uses <c>ArrayList&lt;T&gt;</c>
        /// </summary>
        MutableList,

        /// <summary>
        /// C#/VB.NET: Expose collections as <c><see cref="System.Collections.Generic.IReadOnlyList{T}"/></c>.<br />
        /// Java: Not supported.
        /// </summary>
        IReadOnlyList,

        /// <summary>
        /// C#/VB.NET: Expose collections as <c>System.Collections.Immutable.ImmutableArray&lt;T&gt;</c>.<br />
        /// Java: Not supported.
        /// </summary>
        /// <remarks><c>ImmutableArray</c> is preferred over <c>ImmutableList</c> when append functionality isn't required.</remarks>
        ImmutableArray
    }

    public enum OutputMembers
    {
        /// <summary>C# and VB.NET: Uses auto-properties. Java: uses getter/setter methods.</summary>
        AsProperties,
        AsPublicFields,
//      AsPublicPropertiesOverPrivateFields // TODO
    }

    public enum JsonLibrary
    {
        /// <summary>Use the <see cref="Newtonsoft.Json.JsonPropertyAttribute"/> on generated C# class properties.</summary>
        NewtonsoftJson,

        /// <summary>Use the <c>[JsonPropertyName]</c> attribute on generated C# class properties.</summary>
        SystemTextJson
    }

    public enum JsonPropertyAttributeUsage
    {
        /// <summary>The <c>[JsonProperty]</c> or <c>[JsonPropertyName]</c> attributes will be applied to all properties.</summary>
        Always,

        /// <summary>The <c>[JsonProperty]</c> or <c>[JsonPropertyName]</c> attributes will only be applied to properties with names that cannot be expressed as C# identifiers.</summary>
        OnlyWhenNecessary
    }

    public class MutableClassConfig
    {
        /// <summary>
        /// When <see langword="true"/>, then all properties for collections are read-only, though the actual collection-type can still be mutable. e.g. <c>public List&lt;String&gt; StringValues { get; }</c><br />
        /// When <see langword="false"/>, then all properties for collections are read-only, though the actual collection-type can still be mutable. e.g. <c>public List&lt;String&gt; StringValues { get; set; }</c><br />
        /// Default is <see langword="false"/>.
        /// </summary>
        public bool ReadOnlyCollectionProperties { get; set; }

        public OutputMembers Members { get; set; } = OutputMembers.AsProperties;
    }

    public static class JsonClassGeneratorConfigExtensions
    {
        /// <summary>
        /// Never returns <see langword="null"/>. Returns either:
        /// <list type="bullet">
        /// <item>&quot;<c>[JsonPropertyName(&quot;<paramref name="field"/>.<see cref="FieldInfo.JsonMemberName"/>&quot;)]</c>&quot; (for <c>System.Text.Json</c>).</item>
        /// <item>&quot;<c>[JsonProperty(&quot;<paramref name="field"/>.<see cref="FieldInfo.JsonMemberName"/>&quot;)]</c>&quot; (for <c>Newtonsoft.Json</c>).</item>
        /// <item>&quot;<c>[property: JsonPropertyName(&quot;<paramref name="field"/>.<see cref="FieldInfo.JsonMemberName"/>&quot;)]</c>&quot; (for <c>System.Text.Json</c>) when <see cref="IJsonClassGeneratorConfig.RecordTypes"/> is <see langword="true"/>.</item>
        /// <item>&quot;<c>[property: JsonProperty(&quot;<paramref name="field"/>.<see cref="FieldInfo.JsonMemberName"/>&quot;)]</c>&quot; (for <c>Newtonsoft.Json</c>) when <see cref="IJsonClassGeneratorConfig.RecordTypes"/> is <see langword="true"/>.</item>
        /// <item>An empty string depending on <paramref name="config"/> and <see cref="FieldInfo.ContainsSpecialChars"/>.</item>
        /// </list>
        /// </summary>
        /// <param name="config">Required. Cannot be <see langword="null"/>.</param>
        /// <param name="field">Required. Cannot be <see langword="null"/>.</param>
        /// <returns></returns>
        public static string GetCSharpJsonAttributeCode(this IJsonClassGeneratorConfig config, FieldInfo field)
        {
            if (config is null) throw new ArgumentNullException(nameof(config));
            if (field is null) throw new ArgumentNullException(nameof(field));

            //

            if (UsePropertyAttribute(config, field))
            {
                bool usingRecordTypes = config.OutputType == OutputTypes.ImmutableRecord;
                string attributeTarget = usingRecordTypes ? "property: " : string.Empty;

                switch(config.AttributeLibrary)
                {
                case JsonLibrary.NewtonsoftJson:
                    return $"[{attributeTarget}JsonProperty(\"{field.JsonMemberName}\")]";

                case JsonLibrary.SystemTextJson:
                    return $"[{attributeTarget}JsonPropertyName(\"{field.JsonMemberName}\")]";

                default:
                    throw new InvalidOperationException("Unrecognized " + nameof(config.AttributeLibrary) + " value: " + config.AttributeLibrary);
                }
            }
            else
            {
                return String.Empty;
            }
        }

        private static bool UsePropertyAttribute(IJsonClassGeneratorConfig config, FieldInfo field)
        {
            switch (config.AttributeUsage)
            {
            case JsonPropertyAttributeUsage.Always:
                return true;

            case JsonPropertyAttributeUsage.OnlyWhenNecessary:
                return field.ContainsSpecialChars;

            default:
                throw new InvalidOperationException("Unrecognized " + nameof(config.AttributeUsage) + " value: " + config.AttributeUsage);
            }
        }

        public static bool HasNamespace(this IJsonClassGeneratorConfig config) => !String.IsNullOrEmpty(config.Namespace);
    }
}
