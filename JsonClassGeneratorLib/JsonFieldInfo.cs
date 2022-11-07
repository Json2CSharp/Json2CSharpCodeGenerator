// Copyright © 2010 Xamasoft

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Xamasoft.JsonClassGenerator 
{
    public class JsonFieldInfo
    {
        public JsonFieldInfo(
            JsonClassGenerator generator,
            string                    jsonMemberName,
            JsonType                  type,
            bool                      usePascalCase,
            IReadOnlyList<object>     examples
        )
        {
            this.generator             = generator       ?? throw new ArgumentNullException(nameof(generator));
            this.JsonMemberName        = jsonMemberName  ?? throw new ArgumentNullException(nameof(jsonMemberName));
            this.MemberName            = jsonMemberName;
            this.ContainsSpecialChars  = IsContainsSpecialChars(this.MemberName);

            if (usePascalCase || this.ContainsSpecialChars)
            {
                this.MemberName = this.MemberName.ToTitleCase();
            }

            this.Type     = type ?? throw new ArgumentNullException(nameof(type));
            this.Examples = examples ?? Array.Empty<object>();
        }

        private readonly JsonClassGenerator generator;

        /// <summary>Pascal-cased.</summary>
        public string                MemberName     { get; }
        /// <summary>Normally camelCased.</summary>
        public string                JsonMemberName { get; }
        public JsonType              Type           { get; }
        public IReadOnlyList<object> Examples       { get; }

        public bool ContainsSpecialChars { get; set; }

        private static bool IsContainsSpecialChars(string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (!char.IsLetterOrDigit(c) && c != '_')
                {
                    return true;
                }
            }

            return false;
        }
   
        public string GetExamplesText()
        {
            return string.Join(separator: ", ", values: this.Examples.Take(5).Select(x => JsonConvert.SerializeObject(x)));
        }

    }
}
