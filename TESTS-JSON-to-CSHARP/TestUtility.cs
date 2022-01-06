using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TESTS_JSON_TO_CSHARP
{
    public static class TestUtility
    {
        private static readonly Regex _spaceRegex = new Regex( "( +)", RegexOptions.Compiled );

        public static string NormalizeOutput(this string testData)
        {
            string testDataCleaned = testData
                .Replace("\r", "")
                .Replace("\n", " ")
 //             .Replace(" ", " ")
                .Replace("\t", " ")

#if DEBUG
                .Replace(",", ",\r\n") // Parameters/arguments
                .Replace("{", "\r\n{\r\n")
                .Replace("}", "\r\n}\r\n")
                .Replace(";", ";\r\n")

                .Replace("\r\n\r\n","\r\n")
                .Replace("\r\n\r\n","\r\n")
#endif
            ;

            testDataCleaned = _spaceRegex.Replace( input: testDataCleaned, m => " " ); // i.e.: replace runs of 2-or-more space characters (0x20) with a single space character.

            testDataCleaned = testDataCleaned.Trim();

            return "\r\n" + testDataCleaned + "\r\n"; // The outer line-breaks are added because VS's Test Explorer's Actual vs. Expected comparison is harder to read without them.
        }
    }
}
