using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamasoft.JsonClassGenerator.CodeWriterConfiguration;
using Xamasoft.JsonClassGenerator.Models;

namespace Xamasoft.JsonClassGenerator.CodeWriters
{
    public class DartCodeWriter : ICodeWriter
    {
        private readonly DartCodeWriterConfig config;

        public DartCodeWriter()
        {
            config = new DartCodeWriterConfig();
        }
        
        public DartCodeWriter(DartCodeWriterConfig config)
        {
            this.config = config;
        }

        public String FileExtension => ".dart";

        public String DisplayName => "Dart";

        public IReadOnlyCollection<String> ReservedKeywords => throw new NotImplementedException();

        public string GetTypeName(JsonType type)
        {
            switch (type.Type)
            {
                //case JsonTypeEnum.Anything: return "object"; // Later, test to do
                case JsonTypeEnum.Array: return GetCollectionTypeName(elementTypeName: this.GetTypeName(type.InternalType), config.CollectionType);
                // case JsonTypeEnum.Dictionary: return "Dictionary<string, " + this.GetTypeName(type.InternalType, config) + ">";
                case JsonTypeEnum.Boolean: return "bool?";
                case JsonTypeEnum.Float: return "double?";
                case JsonTypeEnum.Integer: return "int?";
                case JsonTypeEnum.Long: return "double?";
                case JsonTypeEnum.Date: return "DateTime?";
                //case JsonTypeEnum.NonConstrained: return "object"; // Later, test to do
                //case JsonTypeEnum.NullableBoolean: return "bool?";
                //case JsonTypeEnum.NullableFloat: return "double?";
                //case JsonTypeEnum.NullableInteger: return "int?";
                //case JsonTypeEnum.NullableLong: return "long?";
                //case JsonTypeEnum.NullableDate: return "DateTime?";
                //case JsonTypeEnum.NullableSomething: return "object";
                case JsonTypeEnum.Object: return type.NewAssignedName + "?";
                case JsonTypeEnum.String: return "String?";
                default: throw new NotSupportedException("Unsupported json type: " + type.Type);
            }
        }

        public String GetTypeFunctionName(JsonType type)
        {
            switch (type.Type)
            {
                case JsonTypeEnum.Anything: return "";
                case JsonTypeEnum.Array: return "[" + type.InternalType.NewAssignedName + ".from_dict(y) for y in {0}]";
                // case JsonTypeEnum.Dictionary: return "Dictionary<string, " + this.GetTypeName(type.InternalType, config) + ">";
                case JsonTypeEnum.Boolean: return "";
                case JsonTypeEnum.Float: return "float({0})";
                case JsonTypeEnum.Integer: return "int({0})";
                case JsonTypeEnum.Long: return "float({0})";
                //case JsonTypeEnum.Date: return "DateTime"; // Do Later
                case JsonTypeEnum.Date: return "str({0})";
                case JsonTypeEnum.NonConstrained: return "";
                // case JsonTypeEnum.NullableBoolean: return "bool?";
                //case JsonTypeEnum.NullableFloat: return "double?";
                //case JsonTypeEnum.NullableInteger: return "int?";
                //case JsonTypeEnum.NullableLong: return "long?";
                //case JsonTypeEnum.NullableDate: return "DateTime?";
                //case JsonTypeEnum.NullableSomething: return "object";
                case JsonTypeEnum.Object: return type.NewAssignedName + ".from_dict({0})";
                case JsonTypeEnum.String: return "str({0})";
                default: throw new NotSupportedException("Unsupported json type: " + type.Type);
            }
        }

        public Boolean IsReservedKeyword(String word)
        {
            throw new NotImplementedException();
        }

        public void WriteClass(StringBuilder sw, JsonType type)
        {
            var className = type.AssignedName;
            
            sw.AppendFormat("class {0} {{{1}", className, Environment.NewLine);
            this.WriteClassMembers(sw, type, "");
            sw.AppendLine("}");
            sw.AppendLine();
        }

        public void WriteClassMembers(StringBuilder sw, JsonType type, String prefix)
        {
            StringBuilder fields = new StringBuilder();
            StringBuilder fromJsonMapping = new StringBuilder();
            StringBuilder toJsonMapping = new StringBuilder();
            
            if (type.Fields.Any())
            {
                fields.Append("{");
            }

            foreach (JsonFieldInfo field in type.Fields)
            {
                string classPropertyName = field.MemberName;
                string propertyAttribute = field.JsonMemberName.RemoveSpecialCharacters().ToCamelCase();
                string originalPropertyAttribute = field.JsonMemberName;

                sw.AppendFormat("    {0} {1};{2}", field.Type.GetTypeName(), propertyAttribute, Environment.NewLine);
                
                fields.Append("this." + propertyAttribute + ", ");
                if (field.Type.Type == JsonTypeEnum.Array)
                {
                    fromJsonMapping.AppendLine(string.Format("        if (json['{0}'] != null) {{", originalPropertyAttribute));
                    fromJsonMapping.AppendLine(string.Format("         {0} = <{1}>[];", propertyAttribute, field.Type.InternalType.NewAssignedName));
                    fromJsonMapping.AppendLine(string.Format("         json['{0}'].forEach((v) {{", originalPropertyAttribute));
                    fromJsonMapping.AppendLine(string.Format("         {0}!.add({1}.fromJson(v));", propertyAttribute, field.Type.InternalType.NewAssignedName));
                    fromJsonMapping.AppendLine("        });");
                    fromJsonMapping.AppendLine("      }");

                    toJsonMapping.AppendLine(string.Format("        data['{0}'] ={0} != null ? {1}!.map((v) => v?.toJson()).toList() : null;", originalPropertyAttribute, propertyAttribute));
                }
                else if (field.Type.Type == JsonTypeEnum.Object)
                {
                    fromJsonMapping.AppendLine(string.Format("        {1} = json['{0}'] != null ? {2}.fromJson(json['{0}']) : null;", originalPropertyAttribute, propertyAttribute, field.Type.GetTypeName()));
                    toJsonMapping.AppendLine(string.Format("        data['{0}'] = {1}!.toJson();", originalPropertyAttribute, propertyAttribute));
                }
                else
                {
                    fromJsonMapping.AppendLine(string.Format("        {1} = json['{0}'];", originalPropertyAttribute, propertyAttribute));
                    toJsonMapping.AppendLine(string.Format("        data['{0}'] = {1};", originalPropertyAttribute, propertyAttribute));
                }
            }

            if (type.Fields.Any())
            {
                // Remove trailing comma
                fields.Length--;
                fields.Length--;
                fields.Append("}");
            }

            if (!config.RemoveConstructors)
            {
                // Create Class Constructor
                sw.AppendLine();
                sw.AppendLine(string.Format("    {0}({1}); {2}", type.AssignedName, fields.ToString(), Environment.NewLine));
            }

            if (type.Fields.Any())
            {
                if (!config.RemoveFromJson)
                {
                    // Add From Json Function
                    sw.AppendLine(string.Format("    {0}.fromJson(Map<String, dynamic> json) {{", type.AssignedName));
                    sw.Append(fromJsonMapping.ToString());
                    sw.AppendLine("    }");

                    sw.AppendLine();
                }

                if (!config.RemoveToJson)
                {
                    // Add To Json Function
                    sw.AppendLine("    Map<String, dynamic> toJson() {");
                    sw.AppendLine("        final Map<String, dynamic> data = Map<String, dynamic>();");
                    sw.Append(toJsonMapping.ToString());
                    sw.AppendLine("        return data;");
                    sw.AppendLine("    }");
                }

            }
        }

        public void WriteDeserializationComment(StringBuilder sw, bool rootIsArray = false)
        {
            return;
        }

        public void WriteFileEnd(StringBuilder sw)
        {
            // sw.Insert(0, "import json" + Environment.NewLine);
            // sw.Insert(0, "from dataclasses import dataclass" + Environment.NewLine);
            // sw.Insert(0, "from typing import Any" + Environment.NewLine);
            // if (sw.ToString().Contains("List"))
            //     sw.Insert(0, string.Format("from typing import List{0}", Environment.NewLine));

            return;
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

        public void WriteFileStart(StringBuilder sw)
        {
            sw.AppendLine("/* ");
            sw.AppendLine("// Example Usage");
            sw.AppendLine("Map<String, dynamic> map = jsonDecode(<myJSONString>);");
            sw.AppendLine("var myRootNode = Root.fromJson(map);");
            sw.AppendLine("*/ ");

            return;
        }

        public void WriteNamespaceEnd(StringBuilder sw, Boolean root)
        {
         
            return;
        }

        public void WriteNamespaceStart(StringBuilder sw, Boolean root)
        {
            return;
        }

        private static string GetCollectionTypeName(string elementTypeName, OutputCollectionType type)
        {
            switch (type)
            {
                //case OutputCollectionType.Array: return elementTypeName + "[]";
                case OutputCollectionType.Array: return elementTypeName + "[]";
                case OutputCollectionType.MutableList: return "List<" + elementTypeName + ">?";
                // case OutputCollectionType.IReadOnlyList: return "IReadOnlyList<" + elementTypeName + ">";
                // case OutputCollectionType.ImmutableArray: return "ImmutableArray<" + elementTypeName + ">";

                default:
                    throw new ArgumentOutOfRangeException(paramName: nameof(type), actualValue: type, message: "Invalid " + nameof(OutputCollectionType) + " enum value.");
            }
        }
    }
}
