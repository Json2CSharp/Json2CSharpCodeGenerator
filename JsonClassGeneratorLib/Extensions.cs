using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Xamasoft.JsonClassGenerator
{
    public static class StringExtension
    {
        public static string ToCamelCase(this string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > 1)
            {
                return char.ToLowerInvariant(str[0]) + str.Substring(1);
            }
            return str.ToLowerInvariant();
        }

        public static string RemoveSpecialCharacters(this string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9]+", "", RegexOptions.Compiled);
        }

        public static string ReplaceSpecialCharacters(this string str, string replaceWith)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9_]+", replaceWith, RegexOptions.Compiled);
        }

        public static string ToTitleCase(this string str)
        {
            StringBuilder sb = new StringBuilder(str.Length);
            Boolean flag = true;

            for (int i = 0; i < str.Length; i++)
            {
                Char c = str[i];
                string specialCaseFirstCharIsNumber = string.Empty;

                // Handle the case where the first character is a number
                if (i == 0 && char.IsDigit(c))
                    specialCaseFirstCharIsNumber = "_" + c;

                if (char.IsLetterOrDigit(c))
                {
                    if (string.IsNullOrEmpty(specialCaseFirstCharIsNumber))
                    {
                        sb.Append(flag ? char.ToUpper(c) : c);
                    }
                    else
                    {
                        sb.Append(flag ? specialCaseFirstCharIsNumber.ToUpper() : specialCaseFirstCharIsNumber);
                    }

                    flag = false;
                }
                else
                {
                    flag = true;
                }
            }

            return sb.ToString();
        }
    }

}
