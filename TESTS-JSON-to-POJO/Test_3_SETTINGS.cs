
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Xamasoft.JsonClassGenerator;
using Xamasoft.JsonClassGenerator.CodeWriters;

using TESTS_JSON_TO_CSHARP;

namespace TESTS_JSON_to_POJO
{

    [TestClass]
    public class Test_3_SETTINGS
    {
        [TestMethod]
        public void UsePascal()
        {
            Assert.Inconclusive(message: "This test is not yet implemented.");
            return;

            string path       = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_3_SETTINGS_INPUT.txt";
            string resultPath = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_3_SETTINGS_OUTPUT.txt";
            string input      = File.ReadAllText(path);

            JavaCodeWriter javaCodeWriter = new JavaCodeWriter();
            JsonClassGenerator jsonClassGenerator = new JsonClassGenerator();
            jsonClassGenerator.CodeWriter = javaCodeWriter;

            string returnVal = jsonClassGenerator.GenerateClasses(input, out string errorMessage).ToString();
            string resultsCompare = File.ReadAllText(resultPath);
            Assert.AreEqual(resultsCompare.NormalizeOutput(), returnVal.NormalizeOutput());
        }

        [TestMethod]
        public void AddJsonAttribute()
        {
            Assert.Inconclusive(message: "This test is not yet implemented.");
            return;

            string path       = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_3_SETTINGS_INPUT.txt";
            string resultPath = Directory.GetCurrentDirectory().Replace("bin\\Debug", "") + @"Test_3_SETTINGS_OUTPUT.txt";
            string input      = File.ReadAllText(path);

            JavaCodeWriter javaCodeWriter = new JavaCodeWriter();
            JsonClassGenerator jsonClassGenerator = new JsonClassGenerator();
            jsonClassGenerator.CodeWriter = javaCodeWriter;

            string returnVal = jsonClassGenerator.GenerateClasses(input, out string errorMessage).ToString();
            string resultsCompare = File.ReadAllText(resultPath);
            Assert.AreEqual(resultsCompare.NormalizeOutput(), returnVal.NormalizeOutput());
        }
    }
}
