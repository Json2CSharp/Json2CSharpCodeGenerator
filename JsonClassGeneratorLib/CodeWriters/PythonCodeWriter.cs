using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamasoft.JsonClassGenerator.CodeWriters
{
    public class PythonCodeWriter : ICodeBuilder
    {
        public String FileExtension => throw new NotImplementedException();

        public String DisplayName => throw new NotImplementedException();

        public IReadOnlyCollection<String> ReservedKeywords => throw new NotImplementedException();

        public String GetTypeName(JsonType type, IJsonClassGeneratorConfig config)
        {
            switch (type.Type)
            {
                case JsonTypeEnum.Anything: return "object";
                case JsonTypeEnum.Array: return GetCollectionTypeName(elementTypeName: this.GetTypeName(type.InternalType, config), config.CollectionType);
                // case JsonTypeEnum.Dictionary: return "Dictionary<string, " + this.GetTypeName(type.InternalType, config) + ">";
                case JsonTypeEnum.Boolean: return "bool";
                case JsonTypeEnum.Float: return "float";
                case JsonTypeEnum.Integer: return "int";
                case JsonTypeEnum.Long: return "float";
                //case JsonTypeEnum.Date: return "DateTime"; // Do Later
                case JsonTypeEnum.Date: return "str";
                case JsonTypeEnum.NonConstrained: return "object";
                // case JsonTypeEnum.NullableBoolean: return "bool?";
                //case JsonTypeEnum.NullableFloat: return "double?";
                //case JsonTypeEnum.NullableInteger: return "int?";
                //case JsonTypeEnum.NullableLong: return "long?";
                //case JsonTypeEnum.NullableDate: return "DateTime?";
                //case JsonTypeEnum.NullableSomething: return "object";
                case JsonTypeEnum.Object: return type.NewAssignedName;
                case JsonTypeEnum.String: return "str";
                default: throw new NotSupportedException("Unsupported json type: " + type.Type);
            }
        }

        public String GetTypeFunctionName(JsonType type, IJsonClassGeneratorConfig config)
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

        public void WriteClass(IJsonClassGeneratorConfig config, StringBuilder sw, JsonType type)
        {
            var className = type.AssignedName;
            
            sw.AppendLine("@dataclass");
            sw.AppendFormat("class {0}:{1}", className, Environment.NewLine);
            this.WriteClassMembers(config, sw, type, "");
            sw.AppendLine();
        }

        public void WriteClassMembers(IJsonClassGeneratorConfig config, StringBuilder sw, JsonType type, String prefix)
        {
            StringBuilder fields = new StringBuilder();
            StringBuilder mappingFunction = new StringBuilder();
            foreach (FieldInfo field in type.Fields)
            {
                string classPropertyName = field.MemberName;
                string propertyAttribute = field.JsonMemberName;
                string internalPropertyAttribute = "_" + propertyAttribute;
                sw.AppendFormat("    {0}: {1}{2}", propertyAttribute, field.Type.GetTypeName(), Environment.NewLine);
                
                string mappingFragment = string.Format("obj.get(\"{0}\")", propertyAttribute);
                string mappingFragment2 = string.Format(GetTypeFunctionName(field.Type, config), mappingFragment);
                string mappingString = string.Format("        {0} = {1}", internalPropertyAttribute, mappingFragment2);
                mappingFunction.AppendLine(mappingString);
                fields.Append(internalPropertyAttribute + ", ");
            }
            
            // Remove trailing comma
            fields.Length--;
            fields.Length--;
            
            // Write Dictionnary Mapping Functions
            sw.AppendLine();
            sw.AppendLine("    @staticmethod");
            sw.AppendLine(string.Format("    def from_dict(obj: Any) -> '{0}':", type.AssignedName));
            sw.Append(mappingFunction.ToString());
            sw.AppendLine(string.Format("        return {0}({1})", type.AssignedName, fields.ToString()));
        }

        public void WriteDeserializationComment(IJsonClassGeneratorConfig config, StringBuilder sw, bool rootIsArray = false)
        {
            return;
        }

        public void WriteFileEnd(IJsonClassGeneratorConfig config, StringBuilder sw)
        {
            sw.Insert(0, "9 json" + Environment.NewLine);
            sw.Insert(0, "from dataclasses import dataclass" + Environment.NewLine);
            sw.Insert(0, "from typing import Any" + Environment.NewLine);
            if (sw.ToString().Contains("List"))
                sw.Insert(0, string.Format("from typing import List{0}", Environment.NewLine));

            sw.AppendLine("# Example Usage");
            sw.AppendLine("# jsonstring = json.loads(myjsonstring)");
            sw.AppendLine("# root = Root.from_dict(jsonstring)");
            return;
        }

        public void WriteFileStart(IJsonClassGeneratorConfig config, StringBuilder sw)
        {
           

            return;
        }

        public void WriteNamespaceEnd(IJsonClassGeneratorConfig config, StringBuilder sw, Boolean root)
        {
         
            return;
        }

        public void WriteNamespaceStart(IJsonClassGeneratorConfig config, StringBuilder sw, Boolean root)
        {
            return;
        }

        private static string GetCollectionTypeName(string elementTypeName, OutputCollectionType type)
        {
            switch (type)
            {
                //case OutputCollectionType.Array: return elementTypeName + "[]";
                case OutputCollectionType.Array: return "List[" + elementTypeName + "]";
                case OutputCollectionType.MutableList: return "List[" + elementTypeName + "]";
                // case OutputCollectionType.IReadOnlyList: return "IReadOnlyList<" + elementTypeName + ">";
                // case OutputCollectionType.ImmutableArray: return "ImmutableArray<" + elementTypeName + ">";

                default:
                    throw new ArgumentOutOfRangeException(paramName: nameof(type), actualValue: type, message: "Invalid " + nameof(OutputCollectionType) + " enum value.");
            }
        }
    }
}
