using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Xamasoft.JsonClassGenerator.CodeWriters
{
    public class TypeScriptCodeWriter : ICodeWriter
    {
        public string FileExtension
        {
            get { return ".ts"; }
        }

        public string DisplayName
        {
            get { return "TypeScript"; }
        }

        public string GetTypeName(JsonType type, IJsonClassGeneratorConfig config)
        {
            switch (type.Type)
            {
                case JsonTypeEnum.Anything: return "any";
                case JsonTypeEnum.String: return "string";
                case JsonTypeEnum.Boolean: return "bool";
                case JsonTypeEnum.Integer:
                case JsonTypeEnum.Long:
                case JsonTypeEnum.Float: return "number";
                case JsonTypeEnum.Date: return "Date";
                case JsonTypeEnum.NullableInteger:
                case JsonTypeEnum.NullableLong:
                case JsonTypeEnum.NullableFloat: return "number";
                case JsonTypeEnum.NullableBoolean: return "bool";
                case JsonTypeEnum.NullableDate: return "Date";
                case JsonTypeEnum.Object: return type.AssignedName;
                case JsonTypeEnum.Array: return GetTypeName(type.InternalType, config) + "[]";
                case JsonTypeEnum.Dictionary: return "{ [key: string]: " + GetTypeName(type.InternalType, config) + "; }";
                case JsonTypeEnum.NullableSomething: return "any";
                case JsonTypeEnum.NonConstrained: return "any";
                default: throw new NotSupportedException("Unsupported type");
            }
        }

        public void WriteClass(IJsonClassGeneratorConfig config, TextWriter sw, JsonType type)
        {
            var prefix = GetNamespace(config, type.IsRoot) != null ? "    " : "";
            var exported = !config.InternalVisibility || config.SecondaryNamespace != null;
            sw.WriteLine(prefix + (exported ? "export " : string.Empty) + "interface " + type.AssignedName + " {");
            foreach (var field in type.Fields)
            {
                var shouldDefineNamespace = type.IsRoot && config.SecondaryNamespace != null && config.Namespace != null && (field.Type.Type == JsonTypeEnum.Object || (field.Type.InternalType != null && field.Type.InternalType.Type == JsonTypeEnum.Object));
                if (config.ExamplesInDocumentation)
                {
                    sw.WriteLine();
                    sw.WriteLine(prefix + "    /**");
                    sw.WriteLine(prefix + "      * Examples: " + field.GetExamplesText());
                    sw.WriteLine(prefix + "      */");
                }


                sw.WriteLine(prefix + "    " + field.JsonMemberName + (IsNullable(field.Type.Type) ? "?" : "") + ": " + (shouldDefineNamespace ? config.SecondaryNamespace + "." : string.Empty) + GetTypeName(field.Type, config) + ";");
            }
            sw.WriteLine(prefix + "}");
            sw.WriteLine();
        }

        private bool IsNullable(JsonTypeEnum type)
        {
            return
                type == JsonTypeEnum.NullableBoolean ||
                type == JsonTypeEnum.NullableDate ||
                type == JsonTypeEnum.NullableFloat ||
                type == JsonTypeEnum.NullableInteger ||
                type == JsonTypeEnum.NullableLong ||
                type == JsonTypeEnum.NullableSomething;
        }

        public void WriteFileStart(IJsonClassGeneratorConfig config, TextWriter sw)
        {
            // foreach (var line in JsonClassGenerator.FileHeader)
            // {
            //     sw.WriteLine("// " + line);
            // }
            // sw.WriteLine();
        }

        public void WriteFileEnd(IJsonClassGeneratorConfig config, TextWriter sw)
        {
        }

        private string GetNamespace(IJsonClassGeneratorConfig config, bool root)
        {
            return root ? config.Namespace : (config.SecondaryNamespace ?? config.Namespace);
        }

        public void WriteNamespaceStart(IJsonClassGeneratorConfig config, TextWriter sw, bool root)
        {
            if (GetNamespace(config, root) != null)
            {

                sw.WriteLine("module " + GetNamespace(config, root) + " {");
                sw.WriteLine();
            }
        }

        public void WriteNamespaceEnd(IJsonClassGeneratorConfig config, TextWriter sw, bool root)
        {
            if (GetNamespace(config, root) != null)
            {
                sw.WriteLine("}");
                sw.WriteLine();
            }
        }

    }
}
