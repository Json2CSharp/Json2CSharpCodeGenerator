using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamasoft.JsonClassGenerator.Models
{
    public enum JsonLibrary
    {
        /// <summary>Use the <see cref="Newtonsoft.Json.JsonPropertyAttribute"/> on generated C# class properties.</summary>
        NewtonsoftJson,

        /// <summary>Use the <c>[JsonPropertyName]</c> attribute on generated C# class properties.</summary>
        SystemTextJson,

        /// <summary>Use the <c>[JsonPropertyName]</c> and <c>[JsonProperty]</c>  attribute on generated C# class properties.</summary>
        NewtonsoftAndSystemTextJson
    }

}
