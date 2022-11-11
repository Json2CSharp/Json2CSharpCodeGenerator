using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamasoft.JsonClassGenerator.Models;

namespace Xamasoft.JsonClassGenerator.CodeWriterConfiguration
{
    public class VisualBasicCodeWriterConfig : BaseCodeWriterConfiguration
    {
        public VisualBasicCodeWriterConfig()
        {
            UsePascalCase = false;
            UseNestedClasses = false;
            OutputType = OutputTypes.MutableClass;
            OutputMembers = OutputMembers.AsProperties;
            ReadOnlyCollectionProperties = false;
            CollectionType = OutputCollectionType.MutableList;
        }

        public VisualBasicCodeWriterConfig(
            bool usePascalCase,
            bool useNestedClasses,
            JsonLibrary attributeLibrary,
            JsonPropertyAttributeUsage attributeUsage,
            OutputTypes outputType,
            OutputMembers members,
            bool readOnlyCollectionProperties,
            OutputCollectionType collectionType)
        {
            this.UsePascalCase = usePascalCase;
            this.UseNestedClasses = useNestedClasses;
            this.AttributeLibrary = attributeLibrary;
            this.AttributeUsage = attributeUsage;
            this.OutputType = outputType;
            this.OutputMembers = members;
            this.ReadOnlyCollectionProperties = readOnlyCollectionProperties;
            CollectionType = collectionType;
        }

        internal bool UsePascalCase { get; set; }
        internal bool UseNestedClasses { get; set; }
        internal JsonLibrary AttributeLibrary { get; set; }
        internal JsonPropertyAttributeUsage AttributeUsage { get; set; }
        internal OutputTypes OutputType { get; set; }
        internal OutputMembers OutputMembers { get; set; }
        internal bool ReadOnlyCollectionProperties { get; set; }
    }
}
