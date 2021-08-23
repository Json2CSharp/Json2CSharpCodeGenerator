using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xamasoft.JsonClassGenerator;
using Xamasoft.JsonClassGenerator.CodeWriters;

namespace TESTS_JSON_TO_CSHARP
{
    [TestClass]
    public class Test_17_RECORD_TYPES
    {
        [TestMethod]
        public void Run()
        {
            string inputPath = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_17_RECORD_TYPES_INPUT.txt";
            string outputPath = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_17_RECORD_TYPES_OUTPUT.txt";
            string input = File.ReadAllText(inputPath);
            string output = File.ReadAllText(outputPath);

            JsonClassGenerator jsonClassGenerator = new JsonClassGenerator
            {
                CodeWriter = new CSharpCodeWriter(),
                RecordTypes = true,
                UseJsonPropertyName = true, // TODO: also test with newtonsoft json
            };

            string actual = jsonClassGenerator.GenerateClasses(input, out _).ToString();

            var outputFixed = output.Replace(Environment.NewLine, "").Replace(" ", "").Replace("\t", "");
            var actualFixed = actual.Replace(Environment.NewLine, "").Replace(" ", "").Replace("\t", "");

            Assert.AreEqual(outputFixed, actualFixed);
        }
    }
}