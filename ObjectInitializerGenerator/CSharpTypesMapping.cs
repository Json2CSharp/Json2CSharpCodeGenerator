using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectInitializerGenerator
{
    internal static class CSharpTypesMapping
    {
        private static readonly Dictionary<string, string> Mappings = new Dictionary<string, string>
        {
            { "string", "\"\"" },
            { "String", "\"\"" },

            { "bool", "true" },
            { "bool?", "true" },
            { "Boolean", "true" },
            { "Boolean?", "true" },

            { "Guid", "Guid.NewGuid()"},

            { "Datetime", "DateTime.Now"},
            { "Datetime?", "DateTime.Now"},
            { "DateTime", "DateTime.Now"},
            { "DateTime?", "DateTime.Now"},

            { "DateTimeOffset", "DateTimeOffset.Now"},
            { "DateTimeOffset?", "DateTimeOffset.Now"},

            { "int", "1"},
            { "int?", "1"},

            { "uint", "1"},
            { "long", "1"},
            { "double", "1"},
            { "Double", "1"},
            { "float", "1"},
            { "decimal", "1"},
            { "byte", "1"},

            { "char", "\'c\'"},

        };

        static internal string Map(string type)
        {
            return Mappings[type];
        }

        static internal string MapArray(ObjectModel type)
        {
            return string.Format("new {0}[]{{{1}}}", type.GenericType, Mappings[type.GenericType]);
        }

        static internal string MapObject(ObjectModel type)
        {
            if (type.PropertyType.Contains("IEnumerable"))
            {
                return string.Format("new {0}()", type.PropertyType.Replace("IEnumerable", "List"));
            }
            return string.Format("new {0}()", type.PropertyType);
        }
    }
}
