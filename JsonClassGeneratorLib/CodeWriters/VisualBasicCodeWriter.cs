using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xamasoft.JsonClassGenerator.CodeWriterConfiguration;
using Xamasoft.JsonClassGenerator.Models;

namespace Xamasoft.JsonClassGenerator.CodeWriters
{
    public class VisualBasicCodeWriter : ICodeWriterss
    {
        public VisualBasicCodeWriter()
        {
            this.config = new VisualBasicCodeWriterConfig();
        }
        public VisualBasicCodeWriter(VisualBasicCodeWriterConfig config)
        {
            this.config = config;
        }

        public string FileExtension
        {
            get { return ".vb"; }
        }

        public string DisplayName
        {
            get { return "Visual Basic .NET"; }
        }

        private const string NoRenameAttribute = "<Obfuscation(Feature:=\"renaming\", Exclude:=true)>";
        private const string NoPruneAttribute = "<Obfuscation(Feature:=\"trigger\", Exclude:=false)>";
        private readonly VisualBasicCodeWriterConfig config;

        public string GetTypeName(JsonType type)
        {
            switch (type.Type)
            {
                case JsonTypeEnum.Anything         : return "Object";
                case JsonTypeEnum.Array            : return GetCollectionTypeName(GetTypeName(type.InternalType), config.CollectionType);
                case JsonTypeEnum.Dictionary       : return "Dictionary(Of String, " + GetTypeName(type.InternalType) + ")";
                case JsonTypeEnum.Boolean          : return "Boolean";
                case JsonTypeEnum.Float            : return "Double";
                case JsonTypeEnum.Integer          : return "Integer";
                case JsonTypeEnum.Long             : return "Long";
                case JsonTypeEnum.Date             : return "DateTime";
                case JsonTypeEnum.NonConstrained   : return "Object";
                case JsonTypeEnum.NullableBoolean  : return "Boolean?";
                case JsonTypeEnum.NullableFloat    : return "Double?";
                case JsonTypeEnum.NullableInteger  : return "Integer?";
                case JsonTypeEnum.NullableLong     : return "Long?";
                case JsonTypeEnum.NullableDate     : return "DateTime?";
                case JsonTypeEnum.NullableSomething: return "Object";
                case JsonTypeEnum.Object           : return type.AssignedName;
                case JsonTypeEnum.String           : return "String";

                default:
                    throw new NotSupportedException("Unsupported json type");
            }
        }

        private static string GetCollectionTypeName(string elementTypeName, OutputCollectionType type)
        {
            switch (type)
            {
            case OutputCollectionType.Array         : return elementTypeName + "()";
            case OutputCollectionType.MutableList   : return "List(Of" + elementTypeName + ")";
            case OutputCollectionType.IReadOnlyList : return "IReadOnlyList(Of" + elementTypeName + ")";
            case OutputCollectionType.ImmutableArray: return "ImmutableArray(Of" + elementTypeName + ")";

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
            return config.ApplyObfuscationAttributes && (config.OutputType == OutputTypes.MutableClass && config.OutputMembers == OutputMembers.AsPublicFields);
        }

        public void WriteClass(TextWriter sw, JsonType type)
        {
            var visibility = config.InternalVisibility ? "Friend" : "Public";

            if (config.UseNestedClasses)
            {
                sw.WriteLine("    {0} Partial Class {1}", visibility, config.MainClass);
                if (!type.IsRoot)
                {
                    if (ShouldApplyNoRenamingAttribute()) sw.WriteLine("        " + NoRenameAttribute);
                    if (ShouldApplyNoPruneAttribute()   ) sw.WriteLine("        " + NoPruneAttribute);
                    sw.WriteLine("        {0} Class {1}", visibility, type.AssignedName);
                }
            }
            else
            {
                if (ShouldApplyNoRenamingAttribute()) sw.WriteLine("    " + NoRenameAttribute);
                if (ShouldApplyNoPruneAttribute()) sw.WriteLine("    " + NoPruneAttribute);
                sw.WriteLine("    {0} Class {1}", visibility, type.AssignedName);
            }

            var prefix = config.UseNestedClasses && !type.IsRoot ? "            " : "        ";

            WriteClassMembers(sw, type, prefix);

            if (config.UseNestedClasses && !type.IsRoot)
                sw.WriteLine("        End Class");

            sw.WriteLine("    End Class");
            sw.WriteLine();
        }

        private void WriteClassMembers(TextWriter sw, JsonType type, string prefix)
        {


            foreach (var field in type.Fields)
            {
                if (config.UsePascalCase || config.ExamplesInDocumentation) sw.WriteLine();

                if (config.ExamplesInDocumentation)
                {
                    sw.WriteLine(prefix + "''' <summary>");
                    sw.WriteLine(prefix + "''' Examples: " + field.GetExamplesText());
                    sw.WriteLine(prefix + "''' </summary>");
                }

                if (config.UsePascalCase)
                {
                    sw.WriteLine(prefix + "<JsonProperty(\"{0}\")>", field.JsonMemberName);
                }

                if (config.OutputMembers == OutputMembers.AsProperties)
                {
                    sw.WriteLine(prefix + "Public Property {1} As {0}", field.Type.GetTypeName(), field.MemberName);
                }
                else
                {
                    sw.WriteLine(prefix + "Public {1} As {0}", field.Type.GetTypeName(), field.MemberName);
                }
            }

        }

        public void WriteFileStart(TextWriter sw)
        {
            sw.WriteLine();
            sw.WriteLine("Imports System");
            sw.WriteLine("Imports System.Collections.Generic");

            if (ShouldApplyNoRenamingAttribute() || ShouldApplyNoPruneAttribute())
            {
                sw.WriteLine("Imports System.Reflection");
            }

            if (config.AttributeLibrary == JsonLibrary.NewtonsoftJson)
            {
                sw.WriteLine("Imports Newtonsoft.Json");
                sw.WriteLine("Imports Newtonsoft.Json.Linq");
            }
            else if (config.AttributeLibrary == JsonLibrary.SystemTextJson)
            {
                sw.WriteLine("Imports System.Text.Json");
            }

            if (!String.IsNullOrWhiteSpace(config.SecondaryNamespace) && !config.UseNestedClasses)
            {
                sw.WriteLine("Imports {0}", config.SecondaryNamespace);
            }
        }

        public void WriteFileEnd(TextWriter sw)
        {
        }

        public void WriteNamespaceStart(TextWriter sw, bool root)
        {
            sw.WriteLine();
            sw.WriteLine("Namespace Global.{0}", root && !config.UseNestedClasses ? config.Namespace : (config.SecondaryNamespace ?? config.Namespace));
            sw.WriteLine();
        }

        public void WriteNamespaceEnd(TextWriter sw, bool root)
        {

            sw.WriteLine("End Namespace");

        }
    }
}