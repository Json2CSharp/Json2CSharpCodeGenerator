using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamasoft.JsonClassGenerator.Models
{
    public enum OutputCollectionType
    {
        /// <summary>Expose collections as <c>T[]</c>.</summary>
        Array,

        /// <summary>
        /// C#/VB.NET: Expose collections as <c><see cref="System.Collections.Generic.List{T}"/></c>.<br />
        /// Java: Uses <c>ArrayList&lt;T&gt;</c>
        /// </summary>
        MutableList,

        /// <summary>
        /// C#/VB.NET: Expose collections as <c><see cref="System.Collections.Generic.IReadOnlyList{T}"/></c>.<br />
        /// Java: Not supported.
        /// </summary>
        IReadOnlyList,

        /// <summary>
        /// C#/VB.NET: Expose collections as <c>System.Collections.Immutable.ImmutableArray&lt;T&gt;</c>.<br />
        /// Java: Not supported.
        /// </summary>
        /// <remarks><c>ImmutableArray</c> is preferred over <c>ImmutableList</c> when append functionality isn't required.</remarks>
        ImmutableArray
    }
}
