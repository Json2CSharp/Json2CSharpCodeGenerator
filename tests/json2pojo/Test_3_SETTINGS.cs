
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
  public class Test_3_SETTINGS{
   
        [TestMethod]
        public void UsePascal()
        {
            string path       = Path.Combine(Environment.CurrentDirectory, "Test_3_SETTINGS_ADDJSONATTRIBUTE_INPUT.txt");
            string resultPath = Path.Combine(Environment.CurrentDirectory, "Test_3_SETTINGS_ADDJSONATTRIBUTE_OUTPUT.txt");

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
        public void AddJsonAttribute()
        {
            string path       = Path.Combine(Environment.CurrentDirectory, "Test_3_SETTINGS_USEPASCAL_INPUT.txt");
            string resultPath = Path.Combine(Environment.CurrentDirectory, "Test_3_SETTINGS_USEPASCAL_OUTPUT.txt");

            string input = File.ReadAllText(path);
            string errorMessage = string.Empty;
            JavaCodeWriter javaCodeWriter = new JavaCodeWriter();
            JsonClassGenerator jsonClassGenerator = new JsonClassGenerator();
            jsonClassGenerator.CodeWriter = javaCodeWriter;
            jsonClassGenerator.UsePascalCase = true;
            string returnVal = jsonClassGenerator.GenerateClasses(input, out errorMessage).ToString();
            string resultsCompare = File.ReadAllText(resultPath);
            Assert.AreEqual(resultsCompare.Replace(Environment.NewLine, "").Replace(" ", "").Replace("\t", ""), returnVal.Replace(Environment.NewLine, "").Replace(" ", "").Replace("\t", ""));
        }
    }
}
