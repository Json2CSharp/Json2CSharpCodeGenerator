
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using TESTS_JSON_TO_CSHARP;

using Xamasoft.JsonClassGenerator;
using Xamasoft.JsonClassGenerator.CodeWriterConfiguration;
using Xamasoft.JsonClassGenerator.CodeWriters;

namespace TESTS_JSON_to_POJO
{
    [TestClass]
    public class Test_2_HANDLE_NUMBERS
    {
        [TestMethod]
        public void Run_camelCase()
        {
            string path       = Directory.GetCurrentDirectory().Replace("bin\\Debug", "")  + @"Test_2_HANDLE_NUMBERS_INPUT.txt";
            string resultPath = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_2_HANDLE_NUMBERS_OUTPUT.txt";
            string input      = File.ReadAllText(path);

            JavaCodeWriter javaCodeWriter = new JavaCodeWriter();
            JsonClassGenerator jsonClassGenerator = new JsonClassGenerator();
            jsonClassGenerator.CodeWriter = javaCodeWriter;

            string returnVal = jsonClassGenerator.GenerateClasses(input, out string errorMessage).ToString();
            string resultsCompare = File.ReadAllText(resultPath);

            string expectedOutput = resultsCompare.NormalizeOutput();
            string actualOutput   = returnVal     .NormalizeOutput();
            Assert.AreEqual(expected: expectedOutput, actual: actualOutput);
        }

        [TestMethod]
        public void Run_PascalCase()
        {
            string path       = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_2_HANDLE_NUMBERS_INPUT1.txt";
            string resultPath = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_2_HANDLE_NUMBERS_OUTPUT1.txt";
            string input      = File.ReadAllText(path);

            JavaCodeWriterConfig config = new JavaCodeWriterConfig();
            config.UsePascalCase = true;
            JavaCodeWriter javaCodeWriter = new JavaCodeWriter(config);

            JsonClassGenerator jsonClassGenerator = new JsonClassGenerator();
            jsonClassGenerator.CodeWriter = javaCodeWriter;

            string returnVal = jsonClassGenerator.GenerateClasses(input, out string errorMessage).ToString();
            string resultsCompare = File.ReadAllText(resultPath);

            string expectedOutput = resultsCompare.NormalizeOutput();
            string actualOutput   = returnVal     .NormalizeOutput();
            Assert.AreEqual(expected: expectedOutput, actual: actualOutput);
        }
    }
}
