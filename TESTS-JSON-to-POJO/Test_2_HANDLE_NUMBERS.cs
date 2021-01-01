
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamasoft.JsonClassGenerator;
using Xamasoft.JsonClassGenerator.CodeWriters;

namespace TESTS_JSON_to_POJO
{

    [TestClass]
  public class Test_2_HANDLE_NUMBERS{
   
        [TestMethod]
        public void Run()
        { 
        string path = Directory.GetCurrentDirectory().Replace("bin\\Debug", "")  + @"Test_2_HANDLE_NUMBERS_INPUT.txt";

string resultPath = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_2_HANDLE_NUMBERS_OUTPUT.txt";            
            string input = File.ReadAllText(path);
            string errorMessage = string.Empty;
			JavaCodeWriter javaCodeWriter = new JavaCodeWriter();
            JsonClassGenerator jsonClassGenerator = new JsonClassGenerator();
			jsonClassGenerator.CodeWriter = javaCodeWriter;
            string returnVal = jsonClassGenerator.GenerateClasses(input, out errorMessage).ToString();
            string resultsCompare = File.ReadAllText(resultPath); 
                Assert.AreEqual(resultsCompare.Replace(Environment.NewLine, "").Replace(" ", "").Replace("\t", ""), returnVal.Replace(Environment.NewLine, "").Replace(" ", "").Replace("\t", ""));
        }


        [TestMethod]
        public void Run2()
        {
            string path = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_2_HANDLE_NUMBERS_INPUT1.txt";

            string resultPath = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_2_HANDLE_NUMBERS_OUTPUT1.txt";
            string input = File.ReadAllText(path);
            string errorMessage = string.Empty;
            JavaCodeWriter javaCodeWriter = new JavaCodeWriter();
            JsonClassGenerator jsonClassGenerator = new JsonClassGenerator();
            jsonClassGenerator.CodeWriter = javaCodeWriter;
            jsonClassGenerator.UsePascalCase = true;
            jsonClassGenerator.UseJsonAttributes = true;
            string returnVal = jsonClassGenerator.GenerateClasses(input, out errorMessage).ToString();
            string resultsCompare = File.ReadAllText(resultPath);
            Assert.AreEqual(resultsCompare.Replace(Environment.NewLine, "").Replace(" ", "").Replace("\t", ""), returnVal.Replace(Environment.NewLine, "").Replace(" ", "").Replace("\t", ""));
        }
    }
}
