using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamasoft.JsonClassGenerator.Models;

namespace Xamasoft.JsonClassGenerator.CodeWriterConfiguration
{
    public class DartCodeWriterConfig : BaseCodeWriterConfiguration
    {
        /// <summary>
        /// The default constructor with default property values
        /// </summary>
        public DartCodeWriterConfig()
        {
            this.RemoveToJson = false;
            this.RemoveFromJson = false;
            this.RemoveConstructors = false;
            this.OutputMembers = OutputMembers.AsPublicFields;
            CollectionType = OutputCollectionType.MutableList;
        }

        public DartCodeWriterConfig(
            bool removeToJson, 
            bool removeFromJson, 
            bool removeConstructors, 
            OutputMembers outputMembers,
            OutputCollectionType collectionType)
        {
            this.RemoveToJson = removeToJson;
            this.RemoveFromJson = removeFromJson;
            this.RemoveConstructors = removeConstructors;
            this.OutputMembers = outputMembers;
            CollectionType = collectionType;
        }

        public OutputMembers OutputMembers { get; set; }
        public bool RemoveToJson { get; set; }
        public bool RemoveFromJson { get; set; }
        public bool RemoveConstructors { get; set; }
    }
}
