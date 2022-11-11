
using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Xamasoft.JsonClassGenerator;
using Xamasoft.JsonClassGenerator.CodeWriterConfiguration;
using Xamasoft.JsonClassGenerator.CodeWriters;
using Xamasoft.JsonClassGenerator.Models;

namespace TESTS_JSON_TO_CSHARP
{
    [TestClass]
    public class Test_0_DIAGNOSTICS
    {
        [TestMethod]
        public void Run()
        {
            // Assert.Inconclusive(message: "This test is not yet implemented.");

            string path       = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_0_DIAGNOSTICS_INPUT.txt";
            string resultPath = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_0_DIAGNOSTICS_OUTPUT.txt";
            string input = File.ReadAllText(path);

            CSharpCodeWriterConfig csharpCodeWriterConfig = new CSharpCodeWriterConfig();
            csharpCodeWriterConfig.UsePascalCase = true;

            CSharpCodeWriter csharpCodeWriter = new CSharpCodeWriter(csharpCodeWriterConfig);

            JsonClassGenerator jsonClassGenerator = new JsonClassGenerator();
            jsonClassGenerator.CodeWriter = csharpCodeWriter;

            string returnVal = jsonClassGenerator.GenerateClasses(input, errorMessage: out _).ToString();
            string resultsCompare = File.ReadAllText(resultPath);

            string expected = resultsCompare.NormalizeOutput();
            string actual   = returnVal     .NormalizeOutput();

            Assert.Inconclusive(message: "This test is just to diagnose issues.");

            // Assert.AreEqual(expected, actual);
        }
    }
}
