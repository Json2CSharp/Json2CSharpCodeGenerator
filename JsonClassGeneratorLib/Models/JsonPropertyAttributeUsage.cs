using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamasoft.JsonClassGenerator.Models
{
    public enum JsonPropertyAttributeUsage
    {
        /// <summary>The <c>[JsonProperty]</c> or <c>[JsonPropertyName]</c> attributes will be applied to all properties.</summary>
        Always,

        /// <summary>The <c>[JsonProperty]</c> or <c>[JsonPropertyName]</c> attributes will only be applied to properties with names that cannot be expressed as C# identifiers.</summary>
        OnlyWhenNecessary
    }
}
