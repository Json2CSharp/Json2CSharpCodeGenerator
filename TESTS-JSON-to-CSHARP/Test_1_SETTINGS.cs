
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Xamasoft.JsonClassGenerator;
using Xamasoft.JsonClassGenerator.CodeWriters;

namespace TESTS_JSON_TO_CSHARP
{

    [TestClass]
    public class Test_1_SETTINGS
    {

        [TestMethod]
        public void Run()
        {
            string path       = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_1_SETTINGS_INPUT.txt";
            string resultPath = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_1_SETTINGS_OUTPUT.txt";
            string input      = File.ReadAllText(path);

            CSharpCodeWriter csharpCodeWriter = new CSharpCodeWriter();
            JsonClassGenerator jsonClassGenerator = new JsonClassGenerator();
            jsonClassGenerator.CodeWriter = csharpCodeWriter;

            jsonClassGenerator.UsePascalCase = true;
            jsonClassGenerator.AttributeLibrary = JsonLibrary.NewtonsoftJson;
            jsonClassGenerator.AttributeUsage = JsonPropertyAttributeUsage.Always;

            string returnVal = jsonClassGenerator.GenerateClasses(input, out string errorMessage).ToString();
            string resultsCompare = File.ReadAllText(resultPath);
            Assert.AreEqual(resultsCompare.NormalizeOutput(), returnVal.NormalizeOutput());
        }
    }
}
