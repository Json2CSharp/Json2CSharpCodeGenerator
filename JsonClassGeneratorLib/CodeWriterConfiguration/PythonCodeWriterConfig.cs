using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamasoft.JsonClassGenerator.Models;

namespace Xamasoft.JsonClassGenerator.CodeWriterConfiguration
{
    public class PythonCodeWriterConfig : BaseCodeWriterConfiguration
    {
        /// <summary>
        /// The default constructor with default property values
        /// </summary>
        public PythonCodeWriterConfig()
        {
            OutputMembers = OutputMembers.AsPublicFields;
        }

        public OutputMembers OutputMembers { get; set; }

    }
}
