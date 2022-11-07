using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamasoft.JsonClassGenerator.Models
{
    public enum OutputTypes
    {
        /// <summary>
        /// C#: Mutable <c>class</c> types.<br />
        /// VB.NET: Mutable <c>Class</c> types.<br />
        /// Java: Mutable Bean <c>class</c>
        /// </summary>
        MutableClass,

        /// <summary>
        /// C#: Immutable <c>class</c> types. Using <c>[JsonConstructor]</c>.<br />
        /// VB.NET: Immutable <c>Class</c> types. Using <c>[JsonConstructor]</c>.<br />
        /// Java: Not yet implemented. TODO.
        /// </summary>
        ImmutableClass,

        /// <summary>
        /// C#: Immutable <c>record</c> types.<br />
        /// VB.NET: Not supported.<br />
        /// Java: Not supported.<br />
        /// </summary>
        ImmutableRecord
    }
}
