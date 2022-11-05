
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TESTS_JSON_TO_CSHARP;
using Xamasoft.JsonClassGenerator;
using Xamasoft.JsonClassGenerator.CodeWriters;

namespace TESTS_JSON_TO_PYTHON
{
    [TestClass]
    public class Test_0_BASIC_SCENARIO
    {
        [TestMethod]
        public void Run()
        {
            string path       = Directory.GetCurrentDirectory().Replace("bin\\Debug\\netcoreapp3.1", "") + @"Test_0_BASIC_SCENARIO_INPUT.txt";
            string resultPath = Directory.GetCurrentDirectory().Replace("bin\\Debug\\netcoreapp3.1", "") + @"Test_0_BASIC_SCENARIO_OUTPUT.txt";
            string input      = File.ReadAllText(path);

            PythonCodeWriter pythonCodeWriter = new PythonCodeWriter();
            JsonClassGenerator jsonClassGenerator = new JsonClassGenerator();
            jsonClassGenerator.CodeWriter = pythonCodeWriter;
            jsonClassGenerator.MutableClasses.Members = OutputMembers.AsPublicFields;

            string returnVal = jsonClassGenerator.GenerateClasses(input, out string errorMessage).ToString();
            string resultsCompare = File.ReadAllText(resultPath);

            string expectedOutput = resultsCompare.NormalizeOutput();
            string actualOutput   = returnVal     .NormalizeOutput();

            Assert.AreEqual(expected: expectedOutput, actual: actualOutput);

        }
    }
}
