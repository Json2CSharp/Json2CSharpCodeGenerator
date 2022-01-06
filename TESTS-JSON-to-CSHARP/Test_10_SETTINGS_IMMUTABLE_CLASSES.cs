using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Xamasoft.JsonClassGenerator;
using Xamasoft.JsonClassGenerator.CodeWriters;

namespace TESTS_JSON_TO_CSHARP
{
    [TestClass]
    public class Test_10_SETTINGS_IMMUTABLE_CLASSES
    {
        [TestMethod]
        public void Run()
        {
            string path       = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_10_SETTINGS_IMMUTABLE_CLASSES_INPUT.txt";
            string resultPath = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_10_SETTINGS_IMMUTABLE_CLASSES_OUTPUT.txt";
            string input      = File.ReadAllText(path);

            JsonClassGenerator jsonClassGenerator = new JsonClassGenerator()
            {
                CodeWriter          = new CSharpCodeWriter(),
                OutputType          = OutputTypes.ImmutableClass,
                AttributeLibrary    = JsonLibrary.NewtonsoftJson,
                AttributeUsage      = JsonPropertyAttributeUsage.Always,
                UsePascalCase       = true,
            };

            string returnVal      = jsonClassGenerator.GenerateClasses(input, errorMessage: out _).ToString();
            string resultsCompare = File.ReadAllText(resultPath);

            string expected = resultsCompare.NormalizeOutput();
            string actual   = returnVal     .NormalizeOutput();
            Assert.AreEqual(expected, actual);
        }
    }
}
