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
    public class JavaCodeWriterConfig : CodeWriterConfigurationBase
    {
        /// <summary>
        /// The default constructor with default property values
        /// </summary>
        public JavaCodeWriterConfig()
        {
            OutputMembers = OutputMembers.AsPublicFields;
            UsePascalCase = false;
            UseNestedClasses = false;
            CollectionType = OutputCollectionType.MutableList;
        }

        public JavaCodeWriterConfig(OutputMembers outputMembers, OutputCollectionType collectionType, bool usePascalCase = false, bool useNestedClasses = false)
        {
            UsePascalCase = usePascalCase;
            OutputMembers = outputMembers;
            UseNestedClasses = useNestedClasses;
            CollectionType = CollectionType;
        }

        public OutputMembers OutputMembers { get; set; }
        public bool UsePascalCase { get; set; }
        public bool UseNestedClasses { get; set; }
    }
}
