using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamasoft.JsonClassGenerator.Models
{
    public enum OutputMembers
    {
        /// <summary>C# and VB.NET: Uses auto-properties. Java: uses getter/setter methods.</summary>
        AsProperties,
        AsPublicFields,
        // AsPublicPropertiesOverPrivateFields // TODO
    }

}
