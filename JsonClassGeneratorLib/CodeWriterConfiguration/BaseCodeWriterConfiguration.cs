using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamasoft.JsonClassGenerator.Models;

namespace Xamasoft.JsonClassGenerator.CodeWriterConfiguration
{
    public class BaseCodeWriterConfiguration
    {
        /// <summary>
        /// The C# <c>namespace</c> or Java <c>package</c> that the generated types will reside in.<br />
        /// <see langword="null"/> by default. If null/empty/whitespace then no enclosing namespace will be written in the output.
        /// </summary>
        public string Namespace { get; set; }
        public bool HasNamespace { get { return !String.IsNullOrEmpty(Namespace); } }

        /// <summary>
        /// The C# <c>namespace</c> or Java <c>package</c> that &quot;secondary&quot; generated types will reside in.<br />
        /// <see langword="null"/> by default.
        /// </summary>
        public string SecondaryNamespace { get; set; }
        public bool ExamplesInDocumentation { get; set; }

        /// <summary>When <see langword="true"/>, then <see cref="System.Reflection.ObfuscationAttribute"/> will be applied to generated types.</summary>
        public bool ApplyObfuscationAttributes { get; set; }


        #region Class Settings
        public string MainClass { get; set; }
        public bool InternalVisibility { get; set; }
        #endregion

        #region List Settings
        public OutputCollectionType CollectionType { get; set; }
        #endregion

    }
}
