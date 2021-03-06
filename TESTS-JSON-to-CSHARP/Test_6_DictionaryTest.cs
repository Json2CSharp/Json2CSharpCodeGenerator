
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
    public class Test_6_DictionaryTest
    {

        [TestMethod]
        public void Run()
        {
            Assert.Inconclusive(message: "This test is not yet implemented.");
            return;

            string path = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_6_DictionaryTest_INPUT.txt";
            string resultPath = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_6_DictionaryTest_OUTPUT.txt";
            string input = File.ReadAllText(path);
            string errorMessage = string.Empty;
            CSharpCodeWriter csharpCodeWriter = new CSharpCodeWriter();
            JsonClassGenerator jsonClassGenerator = new JsonClassGenerator();
            jsonClassGenerator.CodeWriter = csharpCodeWriter;
            string returnVal = jsonClassGenerator.GenerateClasses(input, out errorMessage).ToString();
            string resultsCompare = File.ReadAllText(resultPath);
            Assert.AreEqual(resultsCompare.Replace(Environment.NewLine, "").Replace(" ", "").Replace("\t", ""), returnVal.Replace(Environment.NewLine, "").Replace(" ", "").Replace("\t", ""));
        }


        [TestMethod]
        public void Run2()
        {
            Assert.Inconclusive(message: "This test is not yet implemented.");
            return;

            string path = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_6_DictionaryTest_INPUT2.txt";
            string resultPath = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_6_DictionaryTest_OUTPUT2.txt";
            string input = File.ReadAllText(path);
            string errorMessage = string.Empty;
            CSharpCodeWriter csharpCodeWriter = new CSharpCodeWriter();
            JsonClassGenerator jsonClassGenerator = new JsonClassGenerator();
            jsonClassGenerator.CodeWriter = csharpCodeWriter;
            string returnVal = jsonClassGenerator.GenerateClasses(input, out errorMessage).ToString();
            string resultsCompare = File.ReadAllText(resultPath);
            Assert.AreEqual(resultsCompare.Replace(Environment.NewLine, "").Replace(" ", "").Replace("\t", ""), returnVal.Replace(Environment.NewLine, "").Replace(" ", "").Replace("\t", ""));
        }

    }
}
