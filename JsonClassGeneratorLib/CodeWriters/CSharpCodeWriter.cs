using System;
using System.Collections.Generic;
using System.Text;

namespace Xamasoft.JsonClassGenerator.CodeWriters
{
    public class CSharpCodeWriter : ICodeBuilder
    {
        public string FileExtension
        {
            get { return ".cs"; }
        }

        public string DisplayName
        {
            get { return "C#"; }
        }

        private const string NoRenameAttribute = "[Obfuscation(Feature = \"renaming\", Exclude = true)]";
        private const string NoPruneAttribute = "[Obfuscation(Feature = \"trigger\", Exclude = false)]";

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

        IReadOnlyCollection<string> ICodeBuilder.ReservedKeywords => _reservedKeywords;

        public string GetTypeName(JsonType type, IJsonClassGeneratorConfig config)
        {
            var arraysAsLists = !config.ExplicitDeserialization;

            switch (type.Type)
            {
                case JsonTypeEnum.Anything         : return "object";
                case JsonTypeEnum.Array            : return arraysAsLists ? "List<" + this.GetTypeName(type.InternalType, config) + ">" : this.GetTypeName(type.InternalType, config) + "[]";
                case JsonTypeEnum.Dictionary       : return "Dictionary<string, " + this.GetTypeName(type.InternalType, config) + ">";
                case JsonTypeEnum.Boolean          : return "bool";
                case JsonTypeEnum.Float            : return "double";
                case JsonTypeEnum.Integer          : return "int";
                case JsonTypeEnum.Long             : return "long";
                case JsonTypeEnum.Date             : return "DateTime";
                case JsonTypeEnum.NonConstrained   : return "object";
                case JsonTypeEnum.NullableBoolean  : return "bool?";
                case JsonTypeEnum.NullableFloat    : return "double?";
                case JsonTypeEnum.NullableInteger  : return "int?";
                case JsonTypeEnum.NullableLong     : return "long?";
                case JsonTypeEnum.NullableDate     : return "DateTime?";
                case JsonTypeEnum.NullableSomething: return "object";
                case JsonTypeEnum.Object           : return type.NewAssignedName;
                case JsonTypeEnum.String           : return "string";
                default: throw new NotSupportedException("Unsupported json type: " + type.Type);
            }
        }

        private bool ShouldApplyNoRenamingAttribute(IJsonClassGeneratorConfig config)
        {
            return config.ApplyObfuscationAttributes && !config.ExplicitDeserialization && !config.UsePascalCase;
        }

        private bool ShouldApplyNoPruneAttribute(IJsonClassGeneratorConfig config)
        {
            return config.ApplyObfuscationAttributes && !config.ExplicitDeserialization && config.UseFields;
        }

        public void WriteFileStart(IJsonClassGeneratorConfig config, StringBuilder sw)
        {
            if (config.UseNamespaces)
            {
                // foreach (var line in JsonClassGenerator.FileHeader)
                // {
                //     sw.AppendFormat("// " + line);
                // }
                sw.AppendLine();
                sw.AppendLine("using System;");
                sw.AppendLine("using System.Collections.Generic;");
                if (this.ShouldApplyNoPruneAttribute(config) || this.ShouldApplyNoRenamingAttribute(config))
                {
                    sw.AppendLine("using System.Reflection;");
                }
                if (!config.ExplicitDeserialization && config.UseJsonAttributes)
                {
                    sw.AppendLine("using Newtonsoft.Json;");
                    sw.AppendLine("using Newtonsoft.Json.Linq;");
                }
                if (!config.ExplicitDeserialization && config.UseJsonPropertyName)
                {
                    sw.AppendLine("System.Text.Json;");
                }
                if (config.ExplicitDeserialization)
                {
                    sw.AppendLine("using JsonCSharpClassGenerator;");
                }
                if (config.SecondaryNamespace != null && config.HasSecondaryClasses && !config.UseNestedClasses)
                {
                    sw.AppendFormat("using {0};", config.SecondaryNamespace);
                }
            }

            if (config.UseNestedClasses)
            {
                sw.AppendFormat("    {0} class {1}", config.InternalVisibility ? "internal" : "public", config.MainClass);
                sw.AppendLine("    {");
            }
        }

        public void WriteFileEnd(IJsonClassGeneratorConfig config, StringBuilder sw)
        {
            if (config.UseNestedClasses)
            {
                sw.AppendLine("    }");
            }
        }

        public void WriteDeserializationComment(IJsonClassGeneratorConfig config, StringBuilder sw)
        {
            if (config.UseJsonPropertyName)
            {
                sw.AppendLine("// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);");
            }
            else
            {
                sw.AppendLine("// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); ");
            }
        }

        public void WriteNamespaceStart(IJsonClassGeneratorConfig config, StringBuilder sw, bool root)
        {
            sw.AppendLine();
            sw.AppendFormat("namespace {0}", root && !config.UseNestedClasses ? config.Namespace : (config.SecondaryNamespace ?? config.Namespace));
            sw.AppendLine("{");
            sw.AppendLine();
        }

        public void WriteNamespaceEnd(IJsonClassGeneratorConfig config, StringBuilder sw, bool root)
        {
            sw.AppendLine("}");
        }

        private static string GetTypeIndent(IJsonClassGeneratorConfig config, bool typeIsRoot)
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

        public void WriteClass(IJsonClassGeneratorConfig config, StringBuilder sw, JsonType type)
        {
            string indentTypes   = GetTypeIndent(config, type.IsRoot);
            string indentMembers = indentTypes   + "    ";
            string indentBodies  = indentMembers + "    ";

            const string visibility = "public";

            var className = type.AssignedName;
            sw.AppendFormat(indentTypes + "{0} class {1}{2}", visibility, className, Environment.NewLine);
            sw.AppendLine  (indentTypes + "{");

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
                if (config.ImmutableClasses)
                {
                    this.WriteClassConstructor(config, sw, type, indentMembers: indentMembers, indentBodies: indentBodies);
                }

                this.WriteClassMembers(config, sw, type, indentMembers);
            }
#if CAN_SUPPRESS
            if (shouldSuppressWarning)
            {
                sw.WriteLine();
                sw.WriteLine("#pragma warning restore 0649");
                sw.WriteLine();
            }
#endif

            if ((!config.UseNestedClasses) || (config.UseNestedClasses && !type.IsRoot))
            {
                sw.AppendLine(indentTypes + "}");
            }

            sw.AppendLine();
        }

        /// <summary>Converts an identifier from JSON into a C#-safe PascalCase identifier.</summary>
        private string GetCSharpPascalCaseName(string name)
        {
            // Check if property is a reserved keyword
            if (this.IsReservedKeyword(name)) name = "@" + name;

            // Check if property name starts with number
            if (!string.IsNullOrEmpty(name) && char.IsDigit(name[0])) name = "_" + name;

            return name;
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

            if      (!Char.IsLetter(name[0]))      name = "_" + name;
            else if (this.IsReservedKeyword(name)) name = "@" + name;

            return name;
        }

        public void WriteClassMembers(IJsonClassGeneratorConfig config, StringBuilder sw, JsonType type, string indentMembers)
        {
            int count = type.Fields.Count;
            int counter = 1;

            foreach (FieldInfo field in type.Fields)
            {
                string classPropertyName = this.GetCSharpPascalCaseName(field.MemberName);

                if (config.ExamplesInDocumentation)
                {
                    sw.AppendFormat(indentMembers + "/// <summary>");
                    sw.AppendFormat(indentMembers + "/// Examples: " + field.GetExamplesText());
                    sw.AppendFormat(indentMembers + "/// </summary>");
                }

                if (config.UseJsonPropertyName)
                {
                    sw.AppendFormat(indentMembers + "[JsonPropertyName(\"{0}\")]{1}", field.JsonMemberName, Environment.NewLine);
                }
                else if (config.UseJsonAttributes || field.ContainsSpecialChars) // If the json Member contains special chars -> add this property
                {
                    sw.AppendFormat(indentMembers + "[JsonProperty(\"{0}\")]{1}", field.JsonMemberName, Environment.NewLine);
                }

                if (config.UseFields)
                {
                    sw.AppendFormat(indentMembers + "public {0} {1};{2}", field.Type.GetTypeName(), classPropertyName, Environment.NewLine);
                }
                else if (config.ImmutableClasses)
                {
                    if(field.Type.Type == JsonTypeEnum.Array)
                    {
                        sw.AppendFormat(indentMembers + "public IReadOnlyList<{0}> {1} {{ get; }}{2}", this.GetTypeName(field.Type.InternalType, config), classPropertyName, Environment.NewLine);
                    }
                    else
                    {
                        sw.AppendFormat(indentMembers + "public {0} {1} {{ get; }}{2}", field.Type.GetTypeName(), classPropertyName, Environment.NewLine);
                    }
                }
                else
                {
                    sw.AppendFormat(indentMembers + "public {0} {1} {{ get; set; }}{2}", field.Type.GetTypeName(), classPropertyName, Environment.NewLine);
                }

                if ((config.UseJsonAttributes || config.UseJsonPropertyName) && count != counter)
                {
                    sw.AppendLine();
                }

                ++counter;
            }

        }

        private void WriteClassConstructor(IJsonClassGeneratorConfig config, StringBuilder sw, JsonType type, string indentMembers, string indentBodies)
        {
            if(type.Fields.Count == 0)
            {
                sw.AppendFormat(indentMembers + "public {0}() {{}}{1}", type.AssignedName, Environment.NewLine);
                return;
            }

            // Constructor signature:

            sw.AppendFormat(indentMembers + "public {0}({1}", type.AssignedName, Environment.NewLine);

            {
                string attributeName = config.UseJsonPropertyName ? "JsonPropertyName" : "JsonProperty";

                FieldInfo lastField = type.Fields[type.Fields.Count-1];

                foreach (FieldInfo field in type.Fields)
                {
                    string ctorParameterName = this.GetCSharpCamelCaseName(field.MemberName);

                    bool isLast = Object.ReferenceEquals(field, lastField);
                    string comma = isLast ? "" : ",";

                    sw.AppendFormat(indentBodies + "[{0}(\"{1}\")] {2} {3}{4}{5}", /*0:*/ attributeName, /*1:*/ field.JsonMemberName, /*2:*/ field.Type.GetTypeName(), /*3:*/ ctorParameterName, /*4:*/ comma, /*5:*/ Environment.NewLine);
                }
            }

            sw.AppendLine(indentMembers + ")");

            // Constructor body:
            sw.AppendLine(indentMembers + "{");

            foreach (FieldInfo field in type.Fields)
            {
                string ctorParameterName = this.GetCSharpCamelCaseName(field.MemberName);
                string classPropertyName = this.GetCSharpPascalCaseName(field.MemberName);

                if(field.Type.Type == JsonTypeEnum.Array && config.ImmutableClasses)
                {
                    string listElementTypeName = this.GetTypeName(field.Type.InternalType, config);
                    sw.AppendFormat(indentBodies + "this.{0} = {1} ?? (IReadOnlyList<{2}>)Array.Empty<{2}>();{3}", /*0:*/ classPropertyName, /*1:*/ ctorParameterName, /*2:*/ listElementTypeName, /*3:*/ Environment.NewLine);
                }
                else
                {
                    sw.AppendFormat(indentBodies + "this.{0} = {1};{2}", /*0:*/ classPropertyName, /*1:*/ ctorParameterName, /*2:*/ Environment.NewLine);
                }
            }

            sw.AppendLine(indentMembers + "}");
            sw.AppendLine();
        }

    }
}
