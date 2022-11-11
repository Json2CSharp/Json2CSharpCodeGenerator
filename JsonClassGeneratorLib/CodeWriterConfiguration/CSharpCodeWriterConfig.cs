using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamasoft.JsonClassGenerator.Models;

namespace Xamasoft.JsonClassGenerator.CodeWriterConfiguration
{
    public class CSharpCodeWriterConfig : BaseCodeWriterConfiguration
    {
        /// <summary>
        /// The default constructor with default property values
        /// </summary>
        public CSharpCodeWriterConfig()
        {
            UsePascalCase = false;
            UseNestedClasses = false;
            OutputType = OutputTypes.MutableClass;
            AttributeLibrary = JsonLibrary.NewtonsoftJson;
            AttributeUsage = JsonPropertyAttributeUsage.OnlyWhenNecessary;
            OutputType = OutputTypes.MutableClass;
            OutputMembers = OutputMembers.AsProperties;
            ReadOnlyCollectionProperties = false;
            CollectionType = OutputCollectionType.MutableList;
            AlwaysUseNullables = false;
            NullValueHandlingIgnore = false;
        }

        public CSharpCodeWriterConfig(
            bool usePascalCase,
            bool useNestedClasses,
            JsonLibrary attributeLibrary,
            JsonPropertyAttributeUsage attributeUsage,
            OutputTypes outputType,
            OutputMembers members,
            bool readOnlyCollectionProperties,
            OutputCollectionType collectionType,
            bool alwaysUseNullables,
            bool nullValueHandlingIgnore)
        {
            this.UsePascalCase = usePascalCase;
            this.UseNestedClasses = useNestedClasses;
            this.AttributeLibrary = attributeLibrary;
            this.AttributeUsage = attributeUsage;
            this.OutputType = outputType;
            this.OutputMembers = members;
            this.ReadOnlyCollectionProperties = readOnlyCollectionProperties;
            this.CollectionType = collectionType;
            this.AlwaysUseNullables = alwaysUseNullables;
            this.NullValueHandlingIgnore = nullValueHandlingIgnore;
        }

        public bool UsePascalCase { get; set; }
        public bool UseNestedClasses { get; set; }
        public JsonLibrary AttributeLibrary { get; set; }
        public bool NullValueHandlingIgnore { get; set; }
        public JsonPropertyAttributeUsage AttributeUsage { get; set; }
        public OutputTypes OutputType { get; set; }
        public OutputMembers OutputMembers { get; set; }
        public bool ReadOnlyCollectionProperties { get; set; }
        public bool AlwaysUseNullables { get; set; }
    }
}
