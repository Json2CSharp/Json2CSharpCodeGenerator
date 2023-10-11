﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Net.Configuration;
using System.Runtime.Serialization.Formatters;
using System.Text;
using Xamasoft.JsonClassGenerator.CodeWriterConfiguration;
using Xamasoft.JsonClassGenerator.Models;

namespace Xamasoft.JsonClassGenerator.CodeWriters
{
    public class CSharpCodeWriter : ICodeWriter
    {
        public CSharpCodeWriter()
        {
            this.config = new CSharpCodeWriterConfig();

        }

        public CSharpCodeWriter(CSharpCodeWriterConfig writerConfig)
        {
            this.config = writerConfig;
        }

        private readonly CSharpCodeWriterConfig config;
        public string FileExtension
        {
            get { return ".cs"; }
        }
        public string DisplayName
        {
            get { return "C#"; }
        }

        private static readonly HashSet<string> _reservedKeywords = new HashSet<string>(comparer: StringComparer.Ordinal) {
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue",
            "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally",
            "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long",
            "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public",
            "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct",
            "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using",
            "virtual", "void", "volatile", "while"
        };

        public bool IsReservedKeyword(string word) => _reservedKeywords.Contains(word ?? string.Empty);

        IReadOnlyCollection<string> ICodeWriter.ReservedKeywords => _reservedKeywords;

        public void WriteFileStart(StringBuilder sw)
        {
            if (config.HasNamespace)
            {
                List<string> importNamespaces = new List<string>()
                {
                    "System",
                    "System.Collections.Generic"
                };

                if (this.ShouldApplyNoPruneAttribute() || this.ShouldApplyNoRenamingAttribute())
                {
                    importNamespaces.Add("System.Reflection");
                }

                switch (config.AttributeLibrary)
                {
                case JsonLibrary.NewtonsoftJson:
                    importNamespaces.Add("Newtonsoft.Json");
                    importNamespaces.Add("Newtonsoft.Json.Linq");
                    break;
                case JsonLibrary.SystemTextJson:
                    importNamespaces.Add("System.Text.Json");
                    break;
                case JsonLibrary.NewtonsoftAndSystemTextJson:
                        importNamespaces.Add("Newtonsoft.Json");
                        importNamespaces.Add("Newtonsoft.Json.Linq");
                        importNamespaces.Add("System.Text.Json");
                    break;

                }

                if (!String.IsNullOrWhiteSpace(config.SecondaryNamespace) && !config.UseNestedClasses)
                {
                    importNamespaces.Add(config.SecondaryNamespace);
                }

                importNamespaces.Sort(CompareNamespacesSystemFirst);

                foreach(string ns in importNamespaces) // NOTE: Using `.Distinct()` after sorting may cause out-of-order results.
                {
                    sw.AppendFormat("using {0};{1}", ns, Environment.NewLine);
                }
            }

            if (config.UseNestedClasses)
            {
                sw.AppendFormat("    {0} class {1}", config.InternalVisibility ? "internal" : "public", config.MainClass);
                sw.AppendLine("    {");
            }
        }

        public void WriteClassesToFile(StringBuilder sw, IEnumerable<JsonType> types, bool rootIsArray = false)
        {
            Boolean inNamespace = false;
            Boolean rootNamespace = false;

            WriteFileStart(sw);
            WriteDeserializationComment(sw, rootIsArray);

            foreach (JsonType type in types)
            {
                if (config.HasNamespace && inNamespace && rootNamespace != type.IsRoot)
                {
                    WriteNamespaceEnd(sw, rootNamespace);
                    inNamespace = false;
                }

                if (config.HasNamespace && !inNamespace)
                {
                    WriteNamespaceStart(sw, type.IsRoot);
                    inNamespace = true;
                    rootNamespace = type.IsRoot;
                }

                WriteClass(sw, type);
            }

            if (config.HasNamespace && inNamespace)
            {
                WriteNamespaceEnd(sw, rootNamespace);
            }

            WriteFileEnd(sw);
        }

        public void WriteFileEnd(StringBuilder sw)
        {
            if (config.UseNestedClasses)
            {
                sw.AppendLine("    }");
            }
        }

        public void WriteDeserializationComment(StringBuilder sw, bool rootIsArray = false)
        {
            String deserializer;
            switch (config.AttributeLibrary)
            {
                case JsonLibrary.NewtonsoftJson:
                    deserializer = "JsonConvert.DeserializeObject";
                    break;
                case JsonLibrary.SystemTextJson:
                    deserializer = "JsonSerializer.Deserialize";
                    break;
                default:
                    return;
            }

            var rootType = rootIsArray ? "List<Root>" : "Root";

            sw.AppendLine($"// Root myDeserializedClass = {deserializer}<{rootType}>(myJsonResponse);");
        }

        public void WriteNamespaceStart(StringBuilder sw, bool root)
        {
            sw.AppendLine();
            sw.AppendFormat("namespace {0}", root && !config.UseNestedClasses ? config.Namespace : (config.SecondaryNamespace ?? config.Namespace));
            sw.AppendLine("{");
            sw.AppendLine();
        }

        public void WriteNamespaceEnd(StringBuilder sw, bool root)
        {
            sw.AppendLine("}");
        }

        public void WriteClass(StringBuilder sw, JsonType type)
        {
            string indentTypes   = GetTypeIndent(type.IsRoot);
            string indentMembers = indentTypes   + "    ";
            string indentBodies  = indentMembers + "    ";

            const string visibility = "public";

            var className = type.AssignedName;
            if (config.OutputType == OutputTypes.ImmutableRecord)
            {
                sw.AppendFormat(indentTypes + "{0} record {1}({2}", visibility, className, Environment.NewLine);
            }
            else
            {
                sw.AppendFormat(indentTypes + "{0} class {1}{2}", visibility, className, Environment.NewLine);
                sw.AppendLine  (indentTypes + "{");
            }

#if CAN_SUPRESS
            var shouldSuppressWarning = config.InternalVisibility && !config.UseProperties && !config.ExplicitDeserialization;
            if (shouldSuppressWarning)
            {
                sw.AppendFormat("#pragma warning disable 0649");
                if (!config.UsePascalCase) sw.AppendLine();
            }
            if (config.ExplicitDeserialization)
            {
                if (config.UseProperties) WriteClassWithPropertiesExplicitDeserialization(sw, type, prefix);
                else WriteClassWithFieldsExplicitDeserialization(sw, type, prefix);
            }
            else
#endif
            {
                if (config.OutputType == OutputTypes.ImmutableClass)
                {
                    this.WriteClassConstructor(sw, type, indentMembers: indentMembers, indentBodies: indentBodies);
                }

                this.WriteClassMembers(sw, type, indentMembers);
            }
#if CAN_SUPPRESS
            if (shouldSuppressWarning)
            {
                sw.WriteLine();
                sw.WriteLine("#pragma warning restore 0649");
                sw.WriteLine();
            }
#endif

            if (config.OutputType == OutputTypes.ImmutableRecord)
            {
                sw.AppendLine(indentTypes + ");");
            }
            else if ((!config.UseNestedClasses) || (config.UseNestedClasses && !type.IsRoot))
            {
                sw.AppendLine(indentTypes + "}");
            }

            sw.AppendLine();
        }

        public void WriteClassMembers(StringBuilder sw, JsonType type, string indentMembers)
        {
            bool first = true;
            foreach (JsonFieldInfo field in type.Fields)
            {
                string classPropertyName = field.MemberName;

                if (config.UsePascalCase)
                    classPropertyName = field.MemberName.ToTitleCase();
                
                classPropertyName = this.CheckSyntax(classPropertyName);

                string propertyAttribute = GetCSharpJsonAttributeCode(field);

                // If we are using record types and this is not the first iteration, add a comma and newline to the previous line
                // this is required because every line except the last should end with a comma
                if (this.config.OutputType == OutputTypes.ImmutableRecord && !first)
                {
                    sw.AppendLine(",");
                }

                if( !first && ( ( propertyAttribute.Length > 0 && this.config.OutputType != OutputTypes.ImmutableRecord ) || this.config.ExamplesInDocumentation) )
                {
                    // If rendering examples/XML comments - or property attributes - then add a newline before the property for readability's sake (except if it's the first property in the class)
                    // For record types, we want all members to be next to each other, unless when using examples
                    sw.AppendLine();
                }

                if (this.config.ExamplesInDocumentation)
                {
                    sw.AppendFormat(indentMembers + "/// <summary>");
                    sw.AppendFormat(indentMembers + "/// Examples: " + field.GetExamplesText());
                    sw.AppendFormat(indentMembers + "/// </summary>");
                    sw.AppendLine();
                }

                if (propertyAttribute.Length > 0)
                {
                    sw.Append(indentMembers);
                    sw.Append(propertyAttribute);

                    if (this.config.OutputType != OutputTypes.ImmutableRecord)
                    {
                        sw.AppendLine();
                    }
                }

                // record types is not compatible with UseFields, so it comes first
                if (config.OutputType == OutputTypes.ImmutableRecord)
                {
                    // NOTE: not adding newlines here, that happens at the start of the loop. We need this so we can lazily add commas at the end.
                    if(field.Type.Type == JsonTypeEnum.Array)
                    {
                        // TODO: Respect config.CollectionType
                        sw.AppendFormat(" IReadOnlyList<{0}> {1}", this.GetTypeName(field.Type.InternalType), classPropertyName);
                    }
                    else
                    {
                        sw.AppendFormat(" {0} {1}", field.Type.GetTypeName(), classPropertyName);
                    }
                }
                else if(this.config.OutputType == OutputTypes.MutableClass)
                {
                    if (config.OutputMembers == OutputMembers.AsPublicFields)
                    {
                        // Render a field like `public int Foobar;`:

                        bool useReadonlyModifier = this.config.OutputType == OutputTypes.ImmutableClass;

                        sw.AppendFormat(indentMembers + "public {0}{1} {2};{3}", useReadonlyModifier ? "readonly " : "", field.Type.GetTypeName(), classPropertyName, Environment.NewLine);
                    }
                    else if( config.OutputMembers == OutputMembers.AsProperties )
                    {
                        var getterSetterPart = "{ get; set; }";

                        bool addCollectionPropertyInitializer =
                            config.ReadOnlyCollectionProperties &&
                            field.Type.IsCollectionType() &&
                            this.config.CollectionType == OutputCollectionType.MutableList;

                        if (addCollectionPropertyInitializer && field.Type.Type == JsonTypeEnum.Array)
                        {
                            getterSetterPart = "{ get; } = new " + field.Type.GetTypeName() + "();";
                        }

                        sw.AppendFormat(indentMembers + "public {0} {1} {2}{3}", field.Type.GetTypeName(), classPropertyName, getterSetterPart, Environment.NewLine);
                    }
                    else
                    {
                        const String PATH = nameof(CSharpCodeWriter.config) + "." + nameof(CSharpCodeWriter.config.OutputType);
                        const String MSG_FMT = "Invalid " + nameof(Models.OutputMembers) + " enum value for " + PATH + ": {0}";
                        throw new InvalidOperationException(MSG_FMT);
                    }
                }
                else if (this.config.OutputType == OutputTypes.ImmutableClass)
                {
                    if(field.Type.Type == JsonTypeEnum.Array)
                    {
                        // TODO: Respect config.CollectionType
                        sw.AppendFormat(indentMembers + "public IReadOnlyList<{0}> {1} {{ get; }}{2}", this.GetTypeName(field.Type.InternalType), classPropertyName, Environment.NewLine);
                    }
                    else
                    {
                        sw.AppendFormat(indentMembers + "public {0} {1} {{ get; }}{2}", field.Type.GetTypeName(), classPropertyName, Environment.NewLine);
                    }
                }
                else
                {
                    throw new InvalidOperationException("Invalid " + nameof(Models.OutputTypes) + " value: " + this.config.OutputType);
                }

                first = false;
            }

            // emit a final newline if we're dealing with record types
            if (this.config.OutputType == OutputTypes.ImmutableRecord)
            {
                sw.AppendLine();
            }
        }

        public string GetTypeName(JsonType type)
        {
            switch (type.Type)
            {
                case JsonTypeEnum.Anything: return "object" + ( config.AlwaysUseNullables ? "?" : "" );
                case JsonTypeEnum.Array: return GetCollectionTypeName(elementTypeName: this.GetTypeName(type.InternalType), config.CollectionType) + ( config.AlwaysUseNullables ? "?" : "" );
                case JsonTypeEnum.Dictionary: return "Dictionary<string, " + this.GetTypeName(type.InternalType) + ">" + ( config.AlwaysUseNullables ? "?" : "" );
                case JsonTypeEnum.Boolean: return config.AlwaysUseNullables ? "bool?" : "bool";
                case JsonTypeEnum.Float: return config.AlwaysUseNullables ? "double?" : "double";
                case JsonTypeEnum.Integer: return config.AlwaysUseNullables ? "int?" : "int";
                case JsonTypeEnum.Long: return config.AlwaysUseNullables ? "long?" : "long";
                case JsonTypeEnum.Date: return config.AlwaysUseNullables ? "DateTime?" : "DateTime";
                case JsonTypeEnum.NonConstrained: return "object" + (config.AlwaysUseNullables ? "?" : "");
                case JsonTypeEnum.NullableBoolean: return "bool?";
                case JsonTypeEnum.NullableFloat: return "double?";
                case JsonTypeEnum.NullableInteger: return "int?";
                case JsonTypeEnum.NullableLong: return "long?";
                case JsonTypeEnum.NullableDate: return "DateTime?";
                case JsonTypeEnum.NullableSomething: return "object?";
                case JsonTypeEnum.Object: return type.NewAssignedName + ( config.AlwaysUseNullables ? "?" : "" );
                case JsonTypeEnum.String: return config.AlwaysUseNullables ? "string?" : "string";
                default: throw new NotSupportedException("Unsupported json type: " + type.Type);
            }
        }

        private void WriteClassConstructor(StringBuilder sw, JsonType type, string indentMembers, string indentBodies)
        {
            // Write an empty constructor on a single-line:
            if (type.Fields.Count == 0)
            {
                sw.AppendFormat(indentMembers + "public {0}() {{}}{1}", type.AssignedName, Environment.NewLine);
                return;
            }

            // Constructor signature:
            {
                switch (config.AttributeLibrary)
                {
                    case JsonLibrary.NewtonsoftJson:
                    case JsonLibrary.SystemTextJson: // Both libraries use the same attribute name: [JsonConstructor]
                        sw.AppendLine(indentMembers + "[JsonConstructor]");
                        break;
                }

                sw.AppendFormat(indentMembers + "public {0}({1}", type.AssignedName, Environment.NewLine);

                JsonFieldInfo lastField = type.Fields[type.Fields.Count-1];

                foreach (JsonFieldInfo field in type.Fields)
                {
                    string ctorParameterName = field.MemberName;
                    if (config.UsePascalCase) { 
                    
                        ctorParameterName = ctorParameterName.ToTitleCase();
                        ctorParameterName = this.GetCSharpCamelCaseName(ctorParameterName);
                    }
                    else if(field.ContainsSpecialChars)
                    {
                        ctorParameterName = this.GetCSharpCamelCaseName(ctorParameterName);
                    }


                    bool isLast = Object.ReferenceEquals(field, lastField);
                    string comma = isLast ? "" : ",";

                    sw.Append(indentBodies);

                    string attribute = GetCSharpJsonAttributeCode(field);
                    if (!string.IsNullOrEmpty(attribute) && config.AttributeLibrary == JsonLibrary.NewtonsoftJson)
                    {
                        sw.Append(attribute);
                        sw.Append(' ');
                    }

                    // e.g. `String foobar,\r\n`
                    sw.AppendFormat("{0} {1}{2}{3}", /*0:*/ field.Type.GetTypeName(), /*1:*/ ctorParameterName, /*2:*/ comma, /*3:*/ Environment.NewLine);
                }
            }

            sw.AppendLine(indentMembers + ")");

            // Constructor body:
            sw.AppendLine(indentMembers + "{");

            foreach (JsonFieldInfo field in type.Fields)
            {
                string ctorName = field.MemberName;
                string classMemberName = field.MemberName;
                if (config.UsePascalCase) {

                    classMemberName = ctorName.ToTitleCase();
                    ctorName = ctorName.ToTitleCase();
                    ctorName = this.GetCSharpCamelCaseName(ctorName);
                }
                else if(field.ContainsSpecialChars && !config.UsePascalCase)
                {
                    classMemberName = this.GetCSharpCamelCaseName(classMemberName);
                    ctorName = this.GetCSharpCamelCaseName(ctorName);
                }

                sw.AppendFormat(indentBodies + "this.{0} = {1};{2}", /*0:*/ classMemberName, /*1:*/ ctorName, /*2:*/ Environment.NewLine);
            }

            sw.AppendLine(indentMembers + "}");
            sw.AppendLine();
        }

        #region Helper Methods
        private static string GetCollectionTypeName(string elementTypeName, OutputCollectionType type)
        {
            switch (type)
            {
                case OutputCollectionType.Array: return elementTypeName + "[]";
                case OutputCollectionType.MutableList: return "List<" + elementTypeName + ">";
                case OutputCollectionType.IReadOnlyList: return "IReadOnlyList<" + elementTypeName + ">";
                case OutputCollectionType.ImmutableArray: return "ImmutableArray<" + elementTypeName + ">";

                default:
                    throw new ArgumentOutOfRangeException(paramName: nameof(type), actualValue: type, message: "Invalid " + nameof(OutputCollectionType) + " enum value.");
            }
        }
        private bool ShouldApplyNoRenamingAttribute()
        {
            return config.ApplyObfuscationAttributes && !config.UsePascalCase;
        }
        private bool ShouldApplyNoPruneAttribute()
        {
            return config.ApplyObfuscationAttributes && ( config.OutputType == OutputTypes.MutableClass && config.OutputMembers == OutputMembers.AsPublicFields );
        }
        private static int CompareNamespacesSystemFirst(string x, string y)
        {
            if (x == "System") return -1;
            if (y == "System") return 1;

            if (x.StartsWith("System.", StringComparison.Ordinal))
            {
                if (y.StartsWith("System.", StringComparison.Ordinal))
                {
                    // Both start with "System." - so compare them normally.
                    return StringComparer.Ordinal.Compare(x, y);
                }
                else
                {
                    // Only `x` starts with "System", so `x` should always come first (i.e. `x < y` or `y > x`).
                    return -1;
                }
            }
            else
            {
                // Only `y` starts with "System", so `y` should always come first (i.e. `x > y` or `y < x`).
                if (y.StartsWith("System.", StringComparison.Ordinal))
                {
                    return 1;
                }
                else
                {
                    // Neither are "System." namespaces - so compare them normally.
                    return StringComparer.Ordinal.Compare(x, y);
                }
            }
        }
        private string GetTypeIndent(bool typeIsRoot)
        {
            if (config.UseNestedClasses)
            {
                if (typeIsRoot)
                {
                    return "    "; // 4x
                }
                else
                {
                    return "        "; // 8x
                }
            }
            else
            {
                return "    "; // 4x
            }
        }
        private string GetCSharpJsonAttributeCode(JsonFieldInfo field)
        {
            if (field is null) throw new ArgumentNullException(nameof(field));

            if (UsePropertyAttribute(field))
            {
                bool usingRecordTypes =  this.config.OutputType == OutputTypes.ImmutableRecord;
                string attributeTarget = usingRecordTypes ? "property: " : string.Empty;

                switch (config.AttributeLibrary)
                {
                    case JsonLibrary.NewtonsoftJson:
                        {
                            if (config.NullValueHandlingIgnore)
                                return $"[{attributeTarget}JsonProperty(\"{field.JsonMemberName}\", NullValueHandling = NullValueHandling.Ignore)]";
                            else
                                return $"[{attributeTarget}JsonProperty(\"{field.JsonMemberName}\")]";
                        }

                    case JsonLibrary.SystemTextJson:
                        return $"[{attributeTarget}JsonPropertyName(\"{field.JsonMemberName}\")]";

                    case JsonLibrary.NewtonsoftAndSystemTextJson:
                        {
                            string newtonsoftAttribute =  $"[{attributeTarget}JsonProperty(\"{field.JsonMemberName}\")]";
                            if (config.NullValueHandlingIgnore) {
                                newtonsoftAttribute = $"[{attributeTarget}JsonProperty(\"{field.JsonMemberName}\", NullValueHandling = NullValueHandling.Ignore)]";
                            }
                                return newtonsoftAttribute + Environment.NewLine + $"        [{attributeTarget}JsonPropertyName(\"{field.JsonMemberName}\")]";
                        }

                    default:
                        throw new InvalidOperationException("Unrecognized " + nameof(config.AttributeLibrary) + " value: " + config.AttributeLibrary);
                }
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>Converts a camelCase identifier from JSON into a C#-safe camelCase identifier.</summary>
        private string GetCSharpCamelCaseName(string camelCaseFromJson)
        {
            if (String.IsNullOrEmpty(camelCaseFromJson)) throw new ArgumentException(message: "Value cannot be null or empty.", paramName: nameof(camelCaseFromJson));

            string name = camelCaseFromJson;

            //

            if (name.Length >= 3)
            {
                if (Char.IsUpper(name[0]) && Char.IsUpper(name[1]) && Char.IsLower(name[2]))
                {
                    // "ABc" --> "abc" // this may be wrong in some cases, if the first two letters are a 2-letter acronym, like "IO".
                    name = name.Substring(startIndex: 0, length: 2).ToLowerInvariant() + name.Substring(startIndex: 2);
                }
                else if (Char.IsUpper(name[0]))
                {
                    // "Abc" --> "abc"
                    // "AbC" --> "abC"
                    name = Char.ToLower(name[0]) + name.Substring(startIndex: 1);
                }
            }
            else if (name.Length == 2)
            {
                if (Char.IsUpper(name[0]))
                {
                    // "AB" --> "ab"
                    // "Ab" --> "ab"
                    name = name.ToLowerInvariant();
                }
            }
            else // Implicit: name.Length == 1
            {
                // "A" --> "a"
                name = name.ToLowerInvariant();
            }

            if (!Char.IsLetter(name[0])) name = "_" + name;
            else if (this.IsReservedKeyword(name)) name = "@" + name;

            return name;
        }
        /// <summary>Converts an identifier from JSON into a C#-safe PascalCase identifier.</summary>
        private string CheckSyntax(string name)
        {
            name = name.ReplaceSpecialCharacters(""); 

            // Check if property is a reserved keyword
            if (this.IsReservedKeyword(name)) name = "@" + name;


            // Check if property name starts with number
            if (!string.IsNullOrEmpty(name) && char.IsDigit(name[0])) name = "_" + name;

            return name;
        }
        private bool UsePropertyAttribute(JsonFieldInfo field)
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
        #endregion

    }
}