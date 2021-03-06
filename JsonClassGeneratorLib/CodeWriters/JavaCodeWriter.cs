using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Xamasoft.JsonClassGenerator.CodeWriters
{
    public class JavaCodeWriter : ICodeBuilder
    {
        public string FileExtension
        {
            get { return ".java"; }
        }

        public string DisplayName
        {
            get { return "Java"; }
        }

        IReadOnlyCollection<string> ICodeBuilder.ReservedKeywords => throw new NotImplementedException();
        bool ICodeBuilder.IsReservedKeyword(string word) => throw new NotImplementedException();

        public string GetTypeName(JsonType type, IJsonClassGeneratorConfig config)
        {
            var arraysAsLists = !config.ExplicitDeserialization;

            switch (type.Type)
            {
                case JsonTypeEnum.Anything: return "Object";
                case JsonTypeEnum.Array: return arraysAsLists ? "List<" + GetTypeName(type.InternalType, config) + ">" : GetTypeName(type.InternalType, config) + "[]";
                case JsonTypeEnum.Dictionary: return "HashMap<String, " + GetTypeName(type.InternalType, config) + ">";
                case JsonTypeEnum.Boolean: return "boolean";
                case JsonTypeEnum.Float: return "double";
                case JsonTypeEnum.Integer: return "int";
                case JsonTypeEnum.NonConstrained: return "Object";
                case JsonTypeEnum.NullableBoolean: return "boolean";
                case JsonTypeEnum.NullableFloat: return "double";
                case JsonTypeEnum.NullableInteger: return "int";
                case JsonTypeEnum.NullableLong: return "long";
                case JsonTypeEnum.NullableDate: return "Date";
                case JsonTypeEnum.Long: return "long";
                case JsonTypeEnum.Date: return "Date";
                case JsonTypeEnum.NullableSomething: return "Object";
                case JsonTypeEnum.Object: return type.NewAssignedName;
                case JsonTypeEnum.String: return "String";
                default: throw new System.NotSupportedException("Unsupported json type");
            }
        }

        public void WriteClass(IJsonClassGeneratorConfig config, StringBuilder sw, JsonType type)
        {
            var visibility = config.InternalVisibility ? "" : "public";

            sw.AppendFormat("{0} class {1}", visibility, type.AssignedName);
            sw.AppendLine("{");

            var prefix = config.UseNestedClasses && !type.IsRoot ? "" : "    ";

            WriteClassMembers(config, sw, type, prefix);

            if (config.UseNestedClasses && !type.IsRoot)
                sw.AppendLine("        }");

            if (!config.UseNestedClasses)
                sw.AppendLine("}");

            sw.AppendLine();
        }

        public void WriteClassMembers(IJsonClassGeneratorConfig config, StringBuilder sw, JsonType type, string prefix)
        {
            foreach (var field in type.Fields)
            {
                if (config.UsePascalCase || config.ExamplesInDocumentation) sw.AppendLine();

                // if (config.UsePascalCase || config.UseJsonAttributes)
                // {
                // 
                // }
                // 
                // 
                // if (config.UseFields)
                // {
                //     sw.AppendFormat(prefix + "@JsonProperty(\"{0}\"){1}", field.JsonMemberName, Environment.NewLine);
                // }
                // else
                // {
                //     sw.AppendFormat(prefix + "@JsonProperty" + "(\"{0}\"){1}", field.JsonMemberName,Environment.NewLine);
                //     sw.AppendFormat(prefix + "public {0} get{1}() {{ \r\t\t return this.{2} \r\t}}", field.Type.GetTypeName(), ChangeFirstChar(field.MemberName), ChangeFirstChar(field.MemberName, false));
                //     sw.AppendFormat(prefix + "public {0} set{1}({0} {2}) {{ \r\t\t this.{2} = {2} \r\t}}", field.Type.GetTypeName(), ChangeFirstChar(field.MemberName), ChangeFirstChar(field.MemberName, false));
                //     sw.AppendFormat(prefix + "{0} {1};", field.Type.GetTypeName(), ChangeFirstChar(field.MemberName, false));
                //     sw.AppendLine();
                // }

                // Check if property name starts with number
                string memberName = field.MemberName;
                if (!string.IsNullOrEmpty(field.MemberName) && char.IsDigit(field.MemberName[0])) memberName = "_" + memberName;

                if (config.UseProperties)
                {
                    sw.AppendFormat(prefix + "@JsonProperty" + "(\"{0}\") {1}", field.JsonMemberName,Environment.NewLine);
                    sw.AppendFormat(prefix + "public {0} get{1}() {{ \r\t\t return this.{2}; }} {3}", field.Type.GetTypeName(), ChangeFirstChar(memberName), ChangeFirstChar(memberName, false), Environment.NewLine);
                    sw.AppendFormat(prefix + "public void set{1}({0} {2}) {{ \r\t\t this.{2} = {2}; }} {3}", field.Type.GetTypeName(), ChangeFirstChar(memberName), ChangeFirstChar(memberName, false), Environment.NewLine);
                    sw.AppendFormat(prefix + "{0} {1};", field.Type.GetTypeName(), ChangeFirstChar(memberName, false));
                    sw.AppendLine();
                }
                else
                {
                    memberName = ChangeFirstChar(memberName, false);
                    if (field.JsonMemberName != memberName)
                        sw.AppendFormat(prefix + "@JsonProperty" + "(\"{0}\") {1}", field.JsonMemberName, Environment.NewLine);
                    sw.AppendFormat(prefix + "public {0} {1};{2}", field.Type.GetTypeName(), memberName, Environment.NewLine);
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

        public void WriteFileStart(IJsonClassGeneratorConfig config, StringBuilder sw)
        {
            // foreach (var line in JsonClassGenerator.FileHeader)
            // {
            //     sw.WriteLine("// " + line);
            // }
        }

        public void WriteFileEnd(IJsonClassGeneratorConfig config, StringBuilder sw)
        {
            if (config.UseNestedClasses)
            {
                sw.AppendLine("    }");
            }
        }

        public void WriteNamespaceStart(IJsonClassGeneratorConfig config, StringBuilder sw, bool root)
        {
            sw.AppendLine();
            sw.AppendFormat("package {0};", root && !config.UseNestedClasses ? config.Namespace : (config.SecondaryNamespace ?? config.Namespace));
            sw.AppendLine();
        }

        public void WriteNamespaceEnd(IJsonClassGeneratorConfig config, StringBuilder sw, bool root)
        {
            sw.AppendLine("}");
        }

        public void WriteDeserializationComment(IJsonClassGeneratorConfig config, StringBuilder sw)
        {
            sw.AppendLine("// import com.fasterxml.jackson.databind.ObjectMapper; // version 2.11.1");
            sw.AppendLine("// import com.fasterxml.jackson.annotation.JsonProperty; // version 2.11.1");
            sw.AppendLine("/* ObjectMapper om = new ObjectMapper();");
            sw.AppendLine("Root root = om.readValue(myJsonString), Root.class); */");
        }
    }
}
