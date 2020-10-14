using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public List<string> ReservedKeywords => new List<string>() { "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while" };

        public string GetTypeName(JsonType type, IJsonClassGeneratorConfig config)
        {
            var arraysAsLists = !config.ExplicitDeserialization;

            switch (type.Type)
            {
                case JsonTypeEnum.Anything: return "object";
                case JsonTypeEnum.Array: return arraysAsLists ? "List<" + GetTypeName(type.InternalType, config) + ">" : GetTypeName(type.InternalType, config) + "[]";
                case JsonTypeEnum.Dictionary: return "Dictionary<string, " + GetTypeName(type.InternalType, config) + ">";
                case JsonTypeEnum.Boolean: return "bool";
                case JsonTypeEnum.Float: return "double";
                case JsonTypeEnum.Integer: return "int";
                case JsonTypeEnum.Long: return "long";
                case JsonTypeEnum.Date: return "DateTime";
                case JsonTypeEnum.NonConstrained: return "object";
                case JsonTypeEnum.NullableBoolean: return "bool?";
                case JsonTypeEnum.NullableFloat: return "double?";
                case JsonTypeEnum.NullableInteger: return "int?";
                case JsonTypeEnum.NullableLong: return "long?";
                case JsonTypeEnum.NullableDate: return "DateTime?";
                case JsonTypeEnum.NullableSomething: return "object";
                case JsonTypeEnum.Object: return type.AssignedName;
                case JsonTypeEnum.String: return "string";
                default: throw new System.NotSupportedException("Unsupported json type");
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
                if (ShouldApplyNoPruneAttribute(config) || ShouldApplyNoRenamingAttribute(config))
                    sw.AppendLine("using System.Reflection;");
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
                    sw.AppendLine("using JsonCSharpClassGenerator;");
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
                sw.AppendLine("// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);");
            else
                sw.AppendLine("// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); ");
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

        public void WriteClass(IJsonClassGeneratorConfig config, StringBuilder sw, JsonType type)
        {
            var visibility = "public";

            sw.AppendFormat("    {0} class {1}", visibility, type.AssignedName);
            sw.AppendLine("    {");

            var prefix = config.UseNestedClasses && !type.IsRoot ? "            " : "        ";

            // var shouldSuppressWarning = config.InternalVisibility && !config.UseProperties && !config.ExplicitDeserialization;
            // if (shouldSuppressWarning)
            // {
            //     sw.AppendFormat("#pragma warning disable 0649");
            //     if (!config.UsePascalCase) sw.AppendLine();
            // }
            // if (config.ExplicitDeserialization)
            // {
            //     if (config.UseProperties) WriteClassWithPropertiesExplicitDeserialization(sw, type, prefix);
            //     else WriteClassWithFieldsExplicitDeserialization(sw, type, prefix);
            // }
            // else
            // {
            WriteClassMembers(config, sw, type, prefix);
            // }

            // if (shouldSuppressWarning)
            // {
            //     sw.WriteLine();
            //     sw.WriteLine("#pragma warning restore 0649");
            //     sw.WriteLine();
            // }


            if (config.UseNestedClasses && !type.IsRoot)
                sw.AppendLine("        }");

            if (!config.UseNestedClasses)
                sw.AppendLine("    }");

            sw.AppendLine();
        }

        public void WriteClassMembers(IJsonClassGeneratorConfig config, StringBuilder sw, JsonType type, string prefix)
        {
            int count = type.Fields.Count;
            int counter = 1;

            foreach (var field in type.Fields)
            {
                string fieldMemberName = field.MemberName;

                // Check if property is a reserved keyword
                if (ReservedKeywords.Contains(fieldMemberName)) fieldMemberName = "@" + fieldMemberName;
                
                if (config.ExamplesInDocumentation)
                {
                    sw.AppendFormat(prefix + "/// <summary>");
                    sw.AppendFormat(prefix + "/// Examples: " + field.GetExamplesText());
                    sw.AppendFormat(prefix + "/// </summary>");
                }

                if (config.UseJsonPropertyName)
                {
                    sw.AppendFormat(prefix + "[JsonPropertyName(\"{0}\")]{1}", field.JsonMemberName, Environment.NewLine);
                }
                else if (config.UseJsonAttributes || field.ContainsSpecialChars) // If the json Member contains special chars -> add this property
                {
                    sw.AppendFormat(prefix + "[JsonProperty(\"{0}\")]{1}", field.JsonMemberName, Environment.NewLine);
                }
               

                if (config.UseFields)
                {
                    sw.AppendFormat(prefix + "public {0} {1}; {2}", field.Type.GetTypeName(), fieldMemberName, Environment.NewLine);
                }
                else
                {
                    sw.AppendFormat(prefix + "public {0} {1} {{ get; set; }} {2}", field.Type.GetTypeName(), fieldMemberName, Environment.NewLine);
                }

                if ((config.UseJsonAttributes || config.UseJsonPropertyName  )&& count != counter)
                {
                    sw.AppendLine();
                }

                ++counter;
            }

        }

    }
}
