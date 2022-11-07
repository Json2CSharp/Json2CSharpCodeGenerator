using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xamasoft.JsonClassGenerator.CodeWriterConfiguration;
using Xamasoft.JsonClassGenerator.Models;

namespace Xamasoft.JsonClassGenerator.CodeWriters
{
    public class JavaCodeWriter : ICodeBuilder
    {
        public JavaCodeWriter()
        {
            this.config = new JavaCodeWriterConfig();
        }
        public JavaCodeWriter(JavaCodeWriterConfig config)
        {
            this.config = config;
        }

        public string FileExtension => ".java";

        public string DisplayName => "Java";

        private static readonly HashSet<string> _reservedKeywords = new HashSet<string>(comparer: StringComparer.Ordinal) {
            // https://en.wikipedia.org/wiki/List_of_Java_keywords
            // `Array.from( document.querySelectorAll('dl > dt > code') ).map( e => '"' + e.textContent + '"' ).sort().join( ", " )`

            "abstract", "assert", "boolean", "break", "byte", "case", "catch", "char", "class", "const", "const", "continue",
            "default", "do", "double", "else", "enum", "extends", "false", "final", "finally", "float", "for", "goto", "goto",
            "if", "implements", "import", "instanceof", "int", "interface", "long", "native", "new", "non-sealed", "null",
            "open", "opens", "package", "permits", "private", "protected", "provides", "public", "record", "return",
            "sealed", "short", "static", "strictfp", "super", "switch", "synchronized",
            "this", "throw", "throws", "to", "transient", "transitive", "true", "try",
            "uses", "var", "void", "volatile", "while", "with", "yield"
        };
        private readonly JavaCodeWriterConfig config;

        IReadOnlyCollection<string> ICodeBuilder.ReservedKeywords => _reservedKeywords;
        public bool IsReservedKeyword(string word) => _reservedKeywords.Contains(word ?? string.Empty);

        public string GetTypeName(JsonType type)
        {
            switch (type.Type)
            {
                case JsonTypeEnum.Anything         : return "Object";
                case JsonTypeEnum.Array            : return GetCollectionTypeName(elementTypeName: GetTypeName(type.InternalType), config.CollectionType);
                case JsonTypeEnum.Dictionary       : return "HashMap<String, " + GetTypeName(type.InternalType) + ">";
                case JsonTypeEnum.Boolean          : return "boolean";
                case JsonTypeEnum.Float            : return "double";
                case JsonTypeEnum.Integer          : return "int";
                case JsonTypeEnum.NonConstrained   : return "Object";
                case JsonTypeEnum.NullableBoolean  : return "boolean";
                case JsonTypeEnum.NullableFloat    : return "double";
                case JsonTypeEnum.NullableInteger  : return "int";
                case JsonTypeEnum.NullableLong     : return "long";
                case JsonTypeEnum.NullableDate     : return "Date";
                case JsonTypeEnum.Long             : return "long";
                case JsonTypeEnum.Date             : return "Date";
                case JsonTypeEnum.NullableSomething: return "Object";
                case JsonTypeEnum.Object           : return type.NewAssignedName;
                case JsonTypeEnum.String           : return "String";

                default:
                throw new NotSupportedException("Unsupported json type");
            }
        }

        private static string GetCollectionTypeName(string elementTypeName, OutputCollectionType type)
        {
            switch (type)
            {
            case OutputCollectionType.Array      : return elementTypeName + "[]";
            case OutputCollectionType.MutableList: return "ArrayList<" + elementTypeName + ">";

            case OutputCollectionType.IReadOnlyList:
            case OutputCollectionType.ImmutableArray:
                throw new NotSupportedException( "Java does not have in-box interfaces for read-only and immutable collections." );

            default:
                throw new ArgumentOutOfRangeException(paramName: nameof(type), actualValue: type, message: "Invalid " + nameof(OutputCollectionType) + " enum value.");
            }
        }

        public void WriteClass(StringBuilder sw, JsonType type)
        {
            var visibility = config.InternalVisibility ? "" : "public";

            sw.AppendFormat("{0} class {1}", visibility, type.AssignedName);
            sw.AppendLine("{");

            var prefix = config.UseNestedClasses && !type.IsRoot ? "" : "    ";

            WriteClassMembers(sw, type, prefix);

            if (config.UseNestedClasses && !type.IsRoot)
                sw.AppendLine("        }");

            if (!config.UseNestedClasses)
                sw.AppendLine("}");

            sw.AppendLine();
        }

        public void WriteClassMembers(StringBuilder sw, JsonType type, string prefix)
        {
            foreach (var field in type.Fields)
            {
                if (config.UsePascalCase || config.ExamplesInDocumentation) sw.AppendLine();

                // Check if property name starts with number
                string memberName = field.MemberName;
                if (!string.IsNullOrEmpty(field.MemberName) && char.IsDigit(field.MemberName[0])) memberName = "_" + memberName;

                if (this.IsReservedKeyword(memberName)) memberName = "my" + memberName;

                if (config.OutputMembers == OutputMembers.AsProperties)
                {
                    sw.AppendFormat(prefix + "@JsonProperty" + "(\"{0}\") {1}", field.JsonMemberName,Environment.NewLine);
                    sw.AppendFormat(prefix + "public {0} get{1}() {{ \r\t\t return this.{2}; }} {3}", field.Type.GetTypeName(), ChangeFirstChar(memberName), ChangeFirstChar(memberName, false), Environment.NewLine);
                    sw.AppendFormat(prefix + "public void set{1}({0} {2}) {{ \r\t\t this.{2} = {2}; }} {3}", field.Type.GetTypeName(), ChangeFirstChar(memberName), ChangeFirstChar(memberName, false), Environment.NewLine);
                    sw.AppendFormat(prefix + "{0} {1};", field.Type.GetTypeName(), ChangeFirstChar(memberName, false));
                    sw.AppendLine();
                }
                else if(config.OutputMembers == OutputMembers.AsPublicFields)
                {
                    memberName = ChangeFirstChar(memberName, false);
                    if (field.JsonMemberName != memberName)
                    {
                        sw.AppendFormat(prefix + "@JsonProperty" + "(\"{0}\") {1}", field.JsonMemberName, Environment.NewLine);
                    }
                    sw.AppendFormat(prefix + "public {0} {1};{2}", field.Type.GetTypeName(), memberName, Environment.NewLine);
                }
                else
                {
                    throw new InvalidOperationException("Unsupported " + nameof(OutputMembers) + " value: " + config.OutputMembers);
                }
            }
        }

        private static string ChangeFirstChar(string value, bool toCaptial = true)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (value.Length == 0)
                return value;
            StringBuilder sb = new StringBuilder();

            sb.Append(toCaptial ? char.ToUpper(value[0]) : char.ToLower(value[0]));
            sb.Append(value.Substring(1));

            return sb.ToString();
        }

        public void WriteFileStart(StringBuilder sw)
        {
            // foreach (var line in JsonClassGenerator.FileHeader)
            // {
            //     sw.WriteLine("// " + line);
            // }
        }

        public void WriteFileEnd(StringBuilder sw)
        {
            if (config.UseNestedClasses)
            {
                sw.AppendLine("    }");
            }
        }

        public void WriteNamespaceStart(StringBuilder sw, bool root)
        {
            sw.AppendLine();
            sw.AppendFormat("package {0};", root && !config.UseNestedClasses ? config.Namespace : (config.SecondaryNamespace ?? config.Namespace));
            sw.AppendLine();
        }

        public void WriteNamespaceEnd(StringBuilder sw, bool root)
        {
            sw.AppendLine("}");
        }

        public void WriteDeserializationComment(StringBuilder sw, bool rootIsArray = false)
        {
            sw.AppendLine("// import com.fasterxml.jackson.databind.ObjectMapper; // version 2.11.1");
            sw.AppendLine("// import com.fasterxml.jackson.annotation.JsonProperty; // version 2.11.1");
            sw.AppendLine("/* ObjectMapper om = new ObjectMapper();");

            if (rootIsArray)
            {
                sw.AppendLine("Root[] root = om.readValue(myJsonString, Root[].class); */");
            }
            else
            {
                sw.AppendLine("Root root = om.readValue(myJsonString, Root.class); */");
            }
        }
    }
}
