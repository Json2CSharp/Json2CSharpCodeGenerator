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
            DoTest(null);
        }

        [TestMethod]
        public void RunNewtonsoft()
        {
            DoTest(false, "_NEWTONSOFT");
        }

        [TestMethod]
        public void RunSystemTextJson()
        {
            DoTest(true, "_SYSTEMTEXTJSON");
        }

        private static void DoTest(bool? useSystemTextJson, string outputFileSuffix = "")
        {
            string inputPath  = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_17_RECORD_TYPES_INPUT.txt";
            string outputPath = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + $@"Test_17_RECORD_TYPES_OUTPUT{outputFileSuffix}.txt";
            string input      = File.ReadAllText(inputPath);
            string output     = File.ReadAllText(outputPath);

            JsonClassGenerator jsonClassGenerator = new JsonClassGenerator
            {
                CodeWriter = new CSharpCodeWriter(),
                RecordTypes = true,
                UseJsonPropertyName = useSystemTextJson == true,
                UseJsonAttributes = useSystemTextJson == false,
            };

            string actual = jsonClassGenerator.GenerateClasses(input, out _).ToString();
            // Console.WriteLine("Actual:\n" + actual);

            var outputFixed = output.NormalizeOutput();
            var actualFixed = actual.NormalizeOutput();

            Assert.AreEqual(expected: outputFixed, actual: actualFixed);
        }
    }
}