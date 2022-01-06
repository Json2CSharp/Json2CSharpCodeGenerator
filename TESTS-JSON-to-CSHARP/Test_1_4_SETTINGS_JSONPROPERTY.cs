using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Xamasoft.JsonClassGenerator;
using Xamasoft.JsonClassGenerator.CodeWriters;

namespace TESTS_JSON_TO_CSHARP
{
    [TestClass]
    public class Test_1_4_SETTINGS_JSONPROPERTY
    {
        [TestMethod]
        public void Run()
        {
            string path       = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_1_4_SETTINGS_JSONPROPERTY_INPUT.txt";
            string resultPath = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_1_4_SETTINGS_JSONPROPERTY_OUTPUT.txt";
            string input      = File.ReadAllText(path);

            CSharpCodeWriter csharpCodeWriter = new CSharpCodeWriter();
            JsonClassGenerator jsonClassGenerator = new JsonClassGenerator();
            jsonClassGenerator.CodeWriter = csharpCodeWriter;
            jsonClassGenerator.AttributeLibrary = JsonLibrary.NewtonsoftJson;
            jsonClassGenerator.AttributeUsage = JsonPropertyAttributeUsage.Always;

            string returnVal = jsonClassGenerator.GenerateClasses(input, out string errorMessage).ToString();
            string resultsCompare = File.ReadAllText(resultPath);

            Assert.AreEqual(expected: resultsCompare.NormalizeOutput(), actual: returnVal.NormalizeOutput());
        }
    }
}
