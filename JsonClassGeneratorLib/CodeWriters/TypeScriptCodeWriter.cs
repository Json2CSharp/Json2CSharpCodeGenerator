using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xamasoft.JsonClassGenerator.CodeWriterConfiguration;
using Xamasoft.JsonClassGenerator.Models;

namespace Xamasoft.JsonClassGenerator.CodeWriters
{
    public class TypeScriptCodeWriter : ICodeWriterss
    {
        private readonly TypeScriptCodeWriterConfig config;

        public TypeScriptCodeWriter()
        {
            this.config = new TypeScriptCodeWriterConfig();
        }

        public TypeScriptCodeWriter(TypeScriptCodeWriterConfig config)
        {
            this.config = config;
        }

        public string FileExtension
        {
            get { return ".ts"; }
        }

        public string DisplayName
        {
            get { return "TypeScript"; }
        }

        public string GetTypeName(JsonType type)
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
                case JsonTypeEnum.Array: return GetTypeName(type.InternalType) + "[]";
                case JsonTypeEnum.Dictionary: return "{ [key: string]: " + GetTypeName(type.InternalType) + "; }";
                case JsonTypeEnum.NullableSomething: return "any";
                case JsonTypeEnum.NonConstrained: return "any";
                default: throw new NotSupportedException("Unsupported type");
            }
        }

        public void WriteClass(TextWriter sw, JsonType type)
        {
            var prefix = GetNamespace(type.IsRoot) != null ? "    " : "";
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


                sw.WriteLine(prefix + "    " + field.JsonMemberName + (IsNullable(field.Type.Type) ? "?" : "") + ": " + (shouldDefineNamespace ? config.SecondaryNamespace + "." : string.Empty) + GetTypeName(field.Type) + ";");
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

        public void WriteFileStart(TextWriter sw)
        {
            // foreach (var line in JsonClassGenerator.FileHeader)
            // {
            //     sw.WriteLine("// " + line);
            // }
            // sw.WriteLine();
        }

        public void WriteFileEnd(TextWriter sw)
        {
        }

        private string GetNamespace(bool root)
        {
            return root ? config.Namespace : (config.SecondaryNamespace ?? config.Namespace);
        }

        public void WriteNamespaceStart(TextWriter sw, bool root)
        {
            if (GetNamespace(root) != null)
            {

                sw.WriteLine("module " + GetNamespace(root) + " {");
                sw.WriteLine();
            }
        }

        public void WriteNamespaceEnd(TextWriter sw, bool root)
        {
            if (GetNamespace(root) != null)
            {
                sw.WriteLine("}");
                sw.WriteLine();
            }
        }

    }
}
