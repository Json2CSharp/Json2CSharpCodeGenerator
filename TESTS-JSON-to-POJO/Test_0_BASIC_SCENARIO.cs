
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using TESTS_JSON_TO_CSHARP;

using Xamasoft.JsonClassGenerator;
using Xamasoft.JsonClassGenerator.CodeWriters;

namespace TESTS_JSON_to_POJO
{

    [TestClass]
  public class Test_0_BASIC_SCENARIO{
   
        [TestMethod]
        public void Run()
        { 
        string path = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_0_BASIC_SCENARIO_INPUT.txt";
            string resultPath = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_0_BASIC_SCENARIO_OUTPUT.txt";            
            string input = File.ReadAllText(path);
             string errorMessage = string.Empty;
			JavaCodeWriter javaCodeWriter = new JavaCodeWriter();
                JsonClassGenerator jsonClassGenerator = new JsonClassGenerator();
            jsonClassGenerator.CodeWriter = javaCodeWriter;

                  string returnVal = jsonClassGenerator.GenerateClasses(input, out errorMessage).ToString();
            string resultsCompare = File.ReadAllText(resultPath); 
                Assert.AreEqual(resultsCompare.Replace(Environment.NewLine, "").Replace(" ", "").Replace("\t", ""), returnVal.Replace(Environment.NewLine, "").Replace(" ", "").Replace("\t", ""));
            string expectedOutput = resultsCompare.NormalizeOutput();
            string actualOutput   = returnVal     .NormalizeOutput();

            Assert.AreEqual(expected: expectedOutput, actual: actualOutput);
        }
    }
}
