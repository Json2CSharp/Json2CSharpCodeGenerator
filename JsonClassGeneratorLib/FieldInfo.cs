// Copyright © 2010 Xamasoft

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Xamasoft.JsonClassGenerator
{
    public class FieldInfo
    {
        public FieldInfo(
            IJsonClassGeneratorConfig generator,
            string                    jsonMemberName,
            JsonType                  type,
            bool                      usePascalCase,
            bool                      useJsonAttributes,
            bool                      useJsonPropertyName,
            IReadOnlyList<object>     examples
        )
        {
            this.generator             = generator       ?? throw new ArgumentNullException(nameof(generator));
            this.JsonMemberName        = jsonMemberName  ?? throw new ArgumentNullException(nameof(jsonMemberName));
            this.MemberName            = jsonMemberName;
            this.ContainsSpecialChars  = IsContainsSpecialChars(this.MemberName);

            if (usePascalCase || useJsonAttributes || useJsonPropertyName || this.ContainsSpecialChars)
            {
                this.MemberName = JsonClassGenerator.ToTitleCase(this.MemberName);
            }
            
            this.Type     = type ?? throw new ArgumentNullException(nameof(type));
            this.Examples = examples ?? Array.Empty<object>();
        }

        private readonly IJsonClassGeneratorConfig generator;

        /// <summary>Pascal-cased.</summary>
        public string                MemberName     { get; }
        /// <summary>Normally camelCased.</summary>
        public string                JsonMemberName { get; }
        public JsonType              Type           { get; }
        public IReadOnlyList<object> Examples       { get; }

        public bool ContainsSpecialChars { get; set; }
        public int  MyProperty           { get; set; }

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

        internal static string ToTitleCase(string text)
        {
            if (text is null) throw new ArgumentNullException(nameof(text));
            if (String.IsNullOrWhiteSpace(text)) return text;

            var sb = new StringBuilder(text.Length);
            var lastCharWasSpecial = true;

            for (int i = 0; i < text.Length; i++)
            {
                var c = text[i];
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(lastCharWasSpecial ? char.ToUpper(c) : c);
                    lastCharWasSpecial = false;
                }
                else
                {
                    lastCharWasSpecial = true;
                }
            }

            return sb.ToString();
        }

        public string GetGenerationCode(string jObject)
        {
            if (jObject is null) throw new ArgumentNullException(nameof(jObject));

            if (this.Type.Type == JsonTypeEnum.Array)
            {
                JsonType innermost = this.Type.GetInnermostType();
                return string.Format(CultureInfo.InvariantCulture, "({1})JsonClassHelper.ReadArray<{5}>(JsonClassHelper.GetJToken<JArray>({0}, \"{2}\"), JsonClassHelper.{3}, typeof({6}))",
                    jObject,
                    this.Type.GetTypeName(),
                    this.JsonMemberName,
                    innermost.GetReaderName(),
                    -1,
                    innermost.GetTypeName(),
                    this.Type.GetTypeName()
                );
            }
            else if (this.Type.Type == JsonTypeEnum.Dictionary)
            {
                return string.Format(CultureInfo.InvariantCulture, "({1})JsonClassHelper.ReadDictionary<{2}>(JsonClassHelper.GetJToken<JObject>({0}, \"{3}\"))",
                    jObject,
                    this.Type.GetTypeName(),
                    this.Type.InternalType.GetTypeName(),
                    this.JsonMemberName,
                    this.Type.GetTypeName()
                );
            }
            else
            {
                return string.Format(CultureInfo.InvariantCulture, "JsonClassHelper.{1}(JsonClassHelper.GetJToken<{2}>({0}, \"{3}\"))",
                    jObject,
                    this.Type.GetReaderName(),
                    this.Type.GetJTokenType(),
                    this.JsonMemberName
                );
            }

        }

        public string GetExamplesText()
        {
            return string.Join(separator: ", ", values: this.Examples.Take(5).Select(x => JsonConvert.SerializeObject(x)));
        }

    }
}
